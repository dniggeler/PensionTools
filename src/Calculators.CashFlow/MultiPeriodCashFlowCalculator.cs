using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Calculators.CashFlow.Models;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace Calculators.CashFlow
{
    public class MultiPeriodCashFlowCalculator : IMultiPeriodCashFlowCalculator
    {
        private readonly IFullTaxCalculator _fullTaxCalculator;
        private readonly IFullCapitalBenefitTaxCalculator _capitalBenefitCalculator;
        private readonly ILogger<MultiPeriodCashFlowCalculator> _logger;

        private readonly MultiPeriodOptions _calculatorOptions;

        public MultiPeriodCashFlowCalculator(
            IOptions<MultiPeriodOptions> options,
            IFullTaxCalculator fullTaxCalculator,
            IFullCapitalBenefitTaxCalculator capitalBenefitCalculator,
            ILogger<MultiPeriodCashFlowCalculator> logger)
        {
            _fullTaxCalculator = fullTaxCalculator;
            _capitalBenefitCalculator = capitalBenefitCalculator;
            _logger = logger;
            _calculatorOptions = options.Value;
        }


        /// <inheritdoc />
        public async Task<MultiPeriodCalculationResult> CalculateAsync(
            MultiPeriodCalculatorPerson person,
            CashFlowDefinitionHolder cashFlowDefinitionHolder)
        {
            IEnumerable<CashFlowModel> cashFlows = cashFlowDefinitionHolder.GenericCashFlowDefinitions
                .SelectMany(d => d.GenerateCashFlow())
                .AggregateCashFlows()
                .ToList();

            int startingYear = cashFlows.Min(item => item.Year);
            int finalYear = cashFlows.Max(item => item.Year);

            List<SinglePeriodCalculationResult> singlePeriodCalculationResults = Enumerable.Empty<SinglePeriodCalculationResult>().ToList();

            // swap every account from begin of year T to T+1 (begin of next year)
            ImmutableDictionary<AccountType, decimal> currentPeriodAccounts = new Dictionary<AccountType, decimal>().ToImmutableDictionary();
            for (int year = startingYear; year <= finalYear; year++)
            {
                int currentYear = year;
                
                List<CashFlowModel> currentYearCashFlows = cashFlows
                    .Where(item => item.Year == currentYear)
                    .ToList();

                List<ClearActionDefinition> currentYearClearCashFlowDefinitions = cashFlowDefinitionHolder
                    .ClearCashFlowDefinitions
                    .Where(item => item.ClearAtYear == currentYear)
                    .ToList();

                // 0. move target asset amount to account if it starts at beginning of the year, and
                //    deduct source asset amount if it is not exogenous.
                foreach (var cashFlow in currentYearCashFlows
                    .Where(item => item.OccurrenceType == OccurrenceType.BeginOfPeriod))
                {
                    currentPeriodAccounts =
                        AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, cashFlow.Target, cashFlow.Amount);

                    currentPeriodAccounts =
                        AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, cashFlow.Source, -cashFlow.Amount);
                }

                // 1. apply begin of year clear cash-flows
                foreach (var clearCashFlowDefinition in currentYearClearCashFlowDefinitions
                    .Where(item => item.OccurrenceType == OccurrenceType.BeginOfPeriod && item.IsTaxable))
                {
                    decimal taxableAmount = currentPeriodAccounts[clearCashFlowDefinition.Flow.Source] *
                                           clearCashFlowDefinition.ClearRatio;
                    
                    currentPeriodAccounts =
                        AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, clearCashFlowDefinition.Flow.Source, -taxableAmount);

                    currentPeriodAccounts =
                        AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, clearCashFlowDefinition.Flow.Target, taxableAmount);

                    var taxPaymentAmount = await CalculateCapitalBenefitsTaxAsync(currentYear, person, taxableAmount);

                    currentPeriodAccounts =
                        AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, AccountType.Wealth, -taxPaymentAmount);
                }

                // 2a. take each account amount, calculate tax, and deduct it from wealth
                decimal totalTaxAmount = decimal.Zero;
                foreach (AccountType accountType in Enum.GetValues<AccountType>())
                {
                    var netAmount = currentPeriodAccounts[accountType];

                    totalTaxAmount += accountType switch
                    {
                        AccountType.Income => await CalculateIncomeTaxAsync(currentYear, person, netAmount),
                        AccountType.Wealth => await CalculateWealthTaxAsync(currentYear, person, netAmount),
                        _ => decimal.Zero
                    };
                }

                // 2b. deduct tax payment from current wealth as tax payments are made after calculating taxes
                currentPeriodAccounts =
                    AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, AccountType.Wealth, -totalTaxAmount);

                // 3. savings quota: take share from current income account and move it to wealth
                decimal newSavings = currentPeriodAccounts[AccountType.Income] * _calculatorOptions.SavingsQuota;

                currentPeriodAccounts =
                    AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, AccountType.Wealth, newSavings);
                currentPeriodAccounts =
                    AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, AccountType.Income, -newSavings);

                // 4. apply end of year clear cash-flows
                foreach (var clearCashFlowDefinition in currentYearClearCashFlowDefinitions
                    .Where(item => item.OccurrenceType == OccurrenceType.EndOfPeriod && item.IsTaxable))
                {
                    decimal taxableAmount = currentPeriodAccounts[clearCashFlowDefinition.Flow.Source] *
                                            clearCashFlowDefinition.ClearRatio;
                    currentPeriodAccounts =
                        AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, clearCashFlowDefinition.Flow.Source, -taxableAmount);

                    currentPeriodAccounts =
                        AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, clearCashFlowDefinition.Flow.Target, taxableAmount);

                    var taxPaymentAmount = await CalculateCapitalBenefitsTaxAsync(currentYear, person, taxableAmount);

                    currentPeriodAccounts =
                        AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, AccountType.Wealth, -taxPaymentAmount);

                }

                // 5. reset flow account types income and exogenous which are swapped to next period
                currentPeriodAccounts = currentPeriodAccounts.Remove(AccountType.Income);
                currentPeriodAccounts = currentPeriodAccounts.Remove(AccountType.Exogenous);
                
                // 6. collect calculation results
                currentPeriodAccounts
                    .Select(pair => new SinglePeriodCalculationResult(currentYear, pair.Value, pair.Key))
                    .Iter(item => singlePeriodCalculationResults.Add(item));
            }

            return new MultiPeriodCalculationResult
            {
                StartingYear = startingYear,
                NumberOfPeriods = finalYear - startingYear + 1,
                Accounts = singlePeriodCalculationResults
            };

            async Task<decimal> CalculateIncomeTaxAsync(
                int currentYear, MultiPeriodCalculatorPerson calculatorPerson, decimal amount)
            {
                TaxPerson taxPerson = new()
                {
                    Name = calculatorPerson.Name,
                    CivilStatus = calculatorPerson.CivilStatus,
                    NumberOfChildren = calculatorPerson.NumberOfChildren,
                    ReligiousGroupType = calculatorPerson.ReligiousGroupType,
                    PartnerReligiousGroupType = calculatorPerson.PartnerReligiousGroupType,
                    TaxableWealth = decimal.Zero,
                    TaxableFederalIncome = amount,
                    TaxableIncome = amount
                };

                Either<string, FullTaxResult> result = await _fullTaxCalculator.CalculateAsync(
                    currentYear, person.MunicipalityId, person.Canton, taxPerson, true);

                return result.Match(
                    Right: r => r.TotalTaxAmount,
                    Left: error =>
                    {
                        _logger.LogError(error);
                        return decimal.Zero;
                    });
            }

            async Task<decimal> CalculateWealthTaxAsync(
                int currentYear, MultiPeriodCalculatorPerson calculatorPerson, decimal amount)
            {
                TaxPerson taxPerson = new()
                {
                    Name = calculatorPerson.Name,
                    CivilStatus = calculatorPerson.CivilStatus,
                    NumberOfChildren = calculatorPerson.NumberOfChildren,
                    ReligiousGroupType = calculatorPerson.ReligiousGroupType,
                    PartnerReligiousGroupType = calculatorPerson.PartnerReligiousGroupType,
                    TaxableWealth = amount,
                    TaxableFederalIncome = decimal.Zero,
                    TaxableIncome = decimal.Zero
                };

                Either<string, FullTaxResult> result = await _fullTaxCalculator.CalculateAsync(
                    currentYear, person.MunicipalityId, person.Canton, taxPerson, true);

                return result.Match(
                    Right: r => r.TotalTaxAmount,
                    Left: error =>
                    {
                        _logger.LogError(error);
                        return decimal.Zero;
                    });
            }

            async Task<decimal> CalculateCapitalBenefitsTaxAsync(
                int currentYear, MultiPeriodCalculatorPerson calculatorPerson, decimal amount)
            {
                CapitalBenefitTaxPerson taxPerson = new()
                {
                    Name = calculatorPerson.Name,
                    CivilStatus = calculatorPerson.CivilStatus,
                    NumberOfChildren = calculatorPerson.NumberOfChildren,
                    ReligiousGroupType = calculatorPerson.ReligiousGroupType,
                    PartnerReligiousGroupType = calculatorPerson.PartnerReligiousGroupType,
                    TaxableBenefits = amount
                };

                Either<string, FullCapitalBenefitTaxResult> result = await _capitalBenefitCalculator.CalculateAsync(
                    currentYear, person.MunicipalityId, person.Canton, taxPerson, true);

                return result.Match(
                    Right: r => r.TotalTaxAmount,
                    Left: error =>
                    {
                        _logger.LogError(error);
                        return decimal.Zero;
                    });
            }
        }

        /// <inheritdoc />
        public Task<MultiPeriodCalculationResult> CalculateAsync(
            int startingYear,
            int numberOfPeriods,
            MultiPeriodCalculatorPerson person,
            CashFlowDefinitionHolder cashFlowDefinitionHolder)
        {
            GenericCashFlowDefinition salaryCashFlowDefinition = new()
            {
                Id = "my salary",
                Name = $"{person.Name} - Lohn",
                InvestmentPeriod = (startingYear, numberOfPeriods),
                Flow = (AccountType.Exogenous, AccountType.Income),
                InitialAmount = person.Income,
                NetGrowthRate = _calculatorOptions.SalaryNetGrowthRate,
                Ordinal = 0,
                RecurringAmount = (person.Income, FrequencyType.Yearly),
                OccurrenceType = OccurrenceType.BeginOfPeriod,
                IsTaxable = true,
                TaxType = TaxType.Income
            };

            GenericCashFlowDefinition wealthCashFlowDefinition = new()
            {
                Id = "my wealth",
                Name = $"{person.Name} - Vermögen",
                InvestmentPeriod = (startingYear, 1),
                Flow = (AccountType.Exogenous, AccountType.Wealth),
                InitialAmount = person.Wealth,
                NetGrowthRate = _calculatorOptions.SalaryNetGrowthRate,
                Ordinal = 0,
                RecurringAmount = (decimal.Zero, FrequencyType.Yearly),
                OccurrenceType = OccurrenceType.BeginOfPeriod,
                IsTaxable = true,
                TaxType = TaxType.Wealth
            };

            GenericCashFlowDefinition pillar3aCashFlowDefinition = new()
            {
                Id = "my 3a account",
                Name = $"{person.Name} - 3a Pillar",
                InitialAmount = person.CapitalBenefits.Pillar3a,
                RecurringAmount = (decimal.Zero, FrequencyType.Yearly),
                Flow = (AccountType.Exogenous, AccountType.CapitalBenefits),
                InvestmentPeriod = (2021, 1),
                IsTaxable = false,
                TaxType = TaxType.Undefined,
                OccurrenceType = OccurrenceType.BeginOfPeriod
            };

            // PK-Einkauf
            GenericCashFlowDefinition pensionPlanCashFlowDefinition = new()
            {
                Id = "my PK account",
                NetGrowthRate = 0,
                Name = "PK-Einkauf",
                InitialAmount = person.CapitalBenefits.PensionPlan,
                RecurringAmount = (decimal.Zero, FrequencyType.Yearly),
                Flow = (AccountType.Exogenous, AccountType.CapitalBenefits),
                InvestmentPeriod = (2021, 1),
                IsTaxable = false,
                TaxType = TaxType.Undefined,
                OccurrenceType = OccurrenceType.BeginOfPeriod
            };

            var extendedDefinitionHolder = cashFlowDefinitionHolder with
            {
                GenericCashFlowDefinitions = new[]
                    {
                        salaryCashFlowDefinition, wealthCashFlowDefinition, pillar3aCashFlowDefinition, pensionPlanCashFlowDefinition
                    }
                    .Concat(cashFlowDefinitionHolder.GenericCashFlowDefinitions)
                    .ToList()
            };

            return CalculateAsync(person, extendedDefinitionHolder);
        }

        private static ImmutableDictionary<AccountType, decimal> AddOrUpdateCurrentPeriodAccounts(
            ImmutableDictionary<AccountType, decimal> currentPeriodAccounts,
            AccountType accountType,
            decimal amount)
        {
            if (currentPeriodAccounts.ContainsKey(accountType))
            {
                currentPeriodAccounts = currentPeriodAccounts.SetItem(
                    accountType,
                    currentPeriodAccounts[accountType] + amount);
            }
            else
            {
                currentPeriodAccounts = currentPeriodAccounts.Add(accountType, amount);
            }

            return currentPeriodAccounts;
        }
    }
}
