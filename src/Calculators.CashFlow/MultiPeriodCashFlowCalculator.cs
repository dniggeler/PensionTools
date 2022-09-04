using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Calculators.CashFlow.Models;
using LanguageExt;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace Calculators.CashFlow
{
    public class MultiPeriodCashFlowCalculator : IMultiPeriodCashFlowCalculator
    {
        private readonly IFullWealthAndIncomeTaxCalculator _fullTaxCalculator;
        private readonly IFullCapitalBenefitTaxCalculator _capitalBenefitCalculator;
        private readonly ILogger<MultiPeriodCashFlowCalculator> _logger;

        public MultiPeriodCashFlowCalculator(
            IFullWealthAndIncomeTaxCalculator fullTaxCalculator,
            IFullCapitalBenefitTaxCalculator capitalBenefitCalculator,
            ILogger<MultiPeriodCashFlowCalculator> logger)
        {
            _fullTaxCalculator = fullTaxCalculator;
            _capitalBenefitCalculator = capitalBenefitCalculator;
            _logger = logger;
        }


        /// <inheritdoc />
        public async Task<Either<string, MultiPeriodCalculationResult>> CalculateAsync(
            MultiPeriodCalculatorPerson person,
            CashFlowDefinitionHolder cashFlowDefinitionHolder,
            MultiPeriodOptions options)
        {
            IEnumerable<CashFlowModel> cashFlows = cashFlowDefinitionHolder.GenericCashFlowDefinitions
                .SelectMany(d => d.GenerateCashFlow())
                .AggregateCashFlows()
                .ToList();

            int startingYear = cashFlows.Min(item => item.DateOfOccurrence.Year);
            int finalYear = cashFlows.Max(item => item.DateOfOccurrence.Year);

            List<SinglePeriodCalculationResult> singlePeriodCalculationResults = Enumerable.Empty<SinglePeriodCalculationResult>().ToList();

            // swap every account from begin of year T to T+1 (begin of next year)
            ImmutableDictionary<AccountType, decimal> currentPeriodAccounts = new Dictionary<AccountType, decimal>().ToImmutableDictionary();
            MultiPeriodCalculatorPerson currentPerson = person;
            for (int year = startingYear; year <= finalYear; year++)
            {

                int currentYear = year;
                
                List<CashFlowModel> currentYearCashFlows = cashFlows
                    .Where(item => item.DateOfOccurrence.Year == currentYear)
                    .ToList();

                List<ClearAccountAction> currentYearClearAccountActions = cashFlowDefinitionHolder
                    .ClearAccountActions
                    .Where(item => item.ClearAtYear == currentYear)
                    .ToList();

                List<ChangeResidenceAction> currentYearChangeResidenceActions = cashFlowDefinitionHolder
                    .ChangeResidenceActions
                    .Where(item => item.ChangeAtYear == currentYear)
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
                foreach (var clearCashFlowDefinition in currentYearClearAccountActions
                    .Where(item => item.OccurrenceType == OccurrenceType.BeginOfPeriod && item.IsTaxable))
                {
                    decimal taxableAmount = currentPeriodAccounts[clearCashFlowDefinition.Flow.Source] *
                                           clearCashFlowDefinition.ClearRatio;
                    
                    currentPeriodAccounts =
                        AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, clearCashFlowDefinition.Flow.Source, -taxableAmount);

                    currentPeriodAccounts =
                        AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, clearCashFlowDefinition.Flow.Target, taxableAmount);

                    var taxPaymentAmount = await CalculateCapitalBenefitsTaxAsync(currentYear, currentPerson, taxableAmount);

                    currentPeriodAccounts =
                        AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, AccountType.Wealth, -taxPaymentAmount);
                }

                // 2a. take each account amount, calculate tax, and deduct it from wealth
                var income = currentPeriodAccounts[AccountType.Income];
                var wealth = currentPeriodAccounts[AccountType.Wealth];

                var totalTaxAmount = await CalculateIncomeAndWealthTaxAsync(currentYear, currentPerson, income, wealth);

                // 2b. deduct tax payment from current wealth as tax payments are made after calculating taxes
                currentPeriodAccounts =
                    AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, AccountType.Wealth, -totalTaxAmount);

                // 3. savings quota: take share from current income account and move it to wealth
                decimal newSavings = currentPeriodAccounts[AccountType.Income] * options.SavingsQuota;

                currentPeriodAccounts =
                    AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, AccountType.Wealth, newSavings);
                currentPeriodAccounts =
                    AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, AccountType.Income, -newSavings);

                // 4. apply end of year clear cash-flows
                foreach (var clearCashFlowDefinition in currentYearClearAccountActions
                    .Where(item => item.OccurrenceType == OccurrenceType.EndOfPeriod && item.IsTaxable))
                {
                    decimal taxableAmount = currentPeriodAccounts[clearCashFlowDefinition.Flow.Source] *
                                            clearCashFlowDefinition.ClearRatio;
                    currentPeriodAccounts =
                        AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, clearCashFlowDefinition.Flow.Source, -taxableAmount);

                    currentPeriodAccounts =
                        AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, clearCashFlowDefinition.Flow.Target, taxableAmount);

                    var taxPaymentAmount = await CalculateCapitalBenefitsTaxAsync(currentYear, currentPerson, taxableAmount);

                    currentPeriodAccounts =
                        AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, AccountType.Wealth, -taxPaymentAmount);

                }

                // 5. reset flow account types income and exogenous which are swapped to next period
                currentPeriodAccounts = currentPeriodAccounts.Remove(AccountType.Income);
                currentPeriodAccounts = currentPeriodAccounts.Remove(AccountType.Exogenous);

                // 6. Change residence
                foreach (ChangeResidenceAction changeResidenceAction in currentYearChangeResidenceActions)
                {
                    currentPerson = currentPerson with
                    {
                        MunicipalityId = changeResidenceAction.DestinationMunicipalityId,
                        Canton = changeResidenceAction.DestinationCanton,
                    };

                    currentPeriodAccounts =
                        AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, AccountType.Wealth, -changeResidenceAction.ChangeCost);
                }

                // 7. compound wealth and capital benefits accounts
                decimal wealthIncrease = currentPeriodAccounts[AccountType.Wealth] * options.WealthNetGrowthRate;
                decimal capitalBenefitsIncrease = currentPeriodAccounts[AccountType.CapitalBenefits] * options.CapitalBenefitsNetGrowthRate;

                currentPeriodAccounts =
                    AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, AccountType.Wealth, wealthIncrease);

                currentPeriodAccounts =
                    AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, AccountType.CapitalBenefits, capitalBenefitsIncrease);

                // 8. collect calculation results
                currentPeriodAccounts
                    .Select(pair => new SinglePeriodCalculationResult
                    {
                        Year = currentYear,
                        Amount = pair.Value,
                        AccountType = pair.Key
                    })
                    .Iter(item => singlePeriodCalculationResults.Add(item));
            }

            return new MultiPeriodCalculationResult
            {
                StartingYear = startingYear,
                NumberOfPeriods = finalYear - startingYear + 1,
                Accounts = singlePeriodCalculationResults
            };

            async Task<decimal> CalculateIncomeAndWealthTaxAsync(
                int currentYear, MultiPeriodCalculatorPerson calculatorPerson, decimal income, decimal wealth)
            {
                TaxPerson taxPerson = new()
                {
                    Name = calculatorPerson.Name,
                    CivilStatus = calculatorPerson.CivilStatus,
                    NumberOfChildren = calculatorPerson.NumberOfChildren,
                    ReligiousGroupType = calculatorPerson.ReligiousGroupType,
                    PartnerReligiousGroupType = calculatorPerson.PartnerReligiousGroupType,
                    TaxableWealth = wealth,
                    TaxableFederalIncome = income,
                    TaxableIncome = income
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
                    TaxableCapitalBenefits = amount
                };

                Either<string, FullCapitalBenefitTaxResult> result = await _capitalBenefitCalculator.CalculateAsync(
                    currentYear, calculatorPerson.MunicipalityId, calculatorPerson.Canton, taxPerson, true);

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
        public Task<Either<string, MultiPeriodCalculationResult>> CalculateAsync(
            int startingYear,
            int numberOfPeriods,
            MultiPeriodCalculatorPerson person,
            CashFlowDefinitionHolder cashFlowDefinitionHolder,
            MultiPeriodOptions options)
        {
            GenericCashFlowDefinition salaryCashFlowDefinition = new()
            {
                Id = "my salary",
                Name = $"{person.Name} - Lohn",
                InvestmentPeriod = new InvestmentPeriod
                { 
                    Year = startingYear,
                    NumberOfPeriods = numberOfPeriods
                },
                Flow = new FlowPair( AccountType.Exogenous, AccountType.Income),
                InitialAmount = decimal.Zero,
                NetGrowthRate = options.SalaryNetGrowthRate,
                Ordinal = 0,
                RecurringInvestment = new RecurringInvestment
                {
                    Amount = person.Income,
                    Frequency = FrequencyType.Yearly,
                },
                OccurrenceType = OccurrenceType.BeginOfPeriod,
                IsTaxable = true,
                TaxType = TaxType.Income
            };

            GenericCashFlowDefinition wealthCashFlowDefinition = new()
            {
                Id = "my wealth",
                Name = $"{person.Name} - Vermögen",
                InvestmentPeriod = new InvestmentPeriod
                {
                    Year = startingYear,
                    NumberOfPeriods = 1
                },
                Flow = new FlowPair(AccountType.Exogenous, AccountType.Wealth),
                InitialAmount = person.Wealth,
                NetGrowthRate = options.WealthNetGrowthRate,
                Ordinal = 0,
                RecurringInvestment = new RecurringInvestment
                {
                    Amount = 0,
                    Frequency = FrequencyType.Yearly,
                },
                OccurrenceType = OccurrenceType.BeginOfPeriod,
                IsTaxable = true,
                TaxType = TaxType.Wealth
            };

            GenericCashFlowDefinition pillar3aCashFlowDefinition = new()
            {
                Id = "my 3a account",
                Name = $"{person.Name} - 3a Pillar",
                InitialAmount = person.CapitalBenefits.Pillar3a,
                RecurringInvestment = new RecurringInvestment
                {
                    Amount = 0,
                    Frequency = FrequencyType.Yearly,
                },
                Flow = new FlowPair(AccountType.Exogenous, AccountType.CapitalBenefits),
                InvestmentPeriod = new InvestmentPeriod
                {
                    Year = startingYear,
                    NumberOfPeriods = 1
                },
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
                RecurringInvestment = new RecurringInvestment
                {
                    Amount = 0,
                    Frequency = FrequencyType.Yearly,
                },
                Flow = new FlowPair(AccountType.Exogenous, AccountType.CapitalBenefits),
                InvestmentPeriod = new InvestmentPeriod
                {
                    Year = startingYear,
                    NumberOfPeriods = 1
                },
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

            return CalculateAsync(person, extendedDefinitionHolder, options);
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
