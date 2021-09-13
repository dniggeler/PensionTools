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
            IReadOnlyCollection<GenericCashFlowDefinition> cashFlowDefinitions,
            Dictionary<AccountType, decimal> initialAccountValues)
        {
            IEnumerable<CashFlowModel> cashFlows = cashFlowDefinitions
                .SelectMany(d => d.GenerateCashFlow())
                .AggregateCashFlows()
                .ToList();

            int startingYear = cashFlows.Min(item => item.Year);
            int finalYear = cashFlows.Max(item => item.Year);

            List<SinglePeriodCalculationResult> singlePeriodCalculationResults = Enumerable.Empty<SinglePeriodCalculationResult>().ToList();

            // swap every account from begin of year T to T+1 (begin of next year)
            ImmutableDictionary<AccountType, decimal> currentPeriodAccounts = initialAccountValues.ToImmutableDictionary();
            for (int year = startingYear; year <= finalYear; year++)
            {
                int currentYear = year;
                List<CashFlowModel> currentYearCashFlows =
                    cashFlows.Where(item => item.Year == currentYear).ToList();

                // 0. move target assets to accounts if they start at beginning of the year.
                foreach (var aggregatedCashFlow in currentYearCashFlows
                    .Where(item => item.OccurrenceType == OccurrenceType.BeginOfPeriod))
                {
                    currentPeriodAccounts =
                        AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, aggregatedCashFlow.Target, aggregatedCashFlow.Amount);
                }

                // 1. calculate taxes for each account type, and deduct from wealth
                foreach (var aggregatedCashFlow in currentYearCashFlows.Where(item => item.IsTaxable))
                {
                    var taxAmount = aggregatedCashFlow.TaxType switch
                    {
                        TaxType.Income => await CalculateIncomeTaxAsync(currentYear, person, aggregatedCashFlow.Amount),
                        TaxType.Wealth => await CalculateWealthTaxAsync(currentYear, person, aggregatedCashFlow.Amount),
                        TaxType.Capital => await CalculateCapitalBenefitsTaxAsync(currentYear, person, aggregatedCashFlow.Amount),
                        _ => decimal.Zero
                    };

                    // deduct taxes from current wealth
                    currentPeriodAccounts =
                        AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, AccountType.Wealth, -taxAmount);
                }

                // 2. add savings from salary (savings rate) to taxable wealth
                foreach (var incomeResult in currentYearCashFlows
                    .Where(item => item.Target == AccountType.Income))
                {
                    decimal newSavings = incomeResult.Amount * _calculatorOptions.SavingsQuota;

                    currentPeriodAccounts =
                        AddOrUpdateCurrentPeriodAccounts(currentPeriodAccounts, AccountType.Wealth, newSavings);
                }

                // 3. move source asset types to target accounts

                // 4. collect calculation results
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
                    currentYear, person.MunicipalityId, person.Canton, taxPerson);

                return result.Match(
                    Right: r => r.TotalTaxAmount,
                    Left: error =>
                    {
                        _logger.LogError(error);
                        return decimal.Zero;
                    });
            }
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

        /// <inheritdoc />
        public Task<MultiPeriodCalculationResult> CalculateAsync(
            int startingYear,
            int numberOfPeriods,
            MultiPeriodCalculatorPerson person,
            IReadOnlyCollection<GenericCashFlowDefinition> cashFlowDefinitions,
            Dictionary<AccountType, decimal> initialAccountValues)
        {
            GenericCashFlowDefinition salaryCashFlowDefinition = new()
            {
                Id = Guid.Empty,
                Name = $"{person.Name} - Lohn",
                InvestmentPeriod = (startingYear, numberOfPeriods),
                Flow = (AccountType.Exogenous, AccountType.Income),
                InitialAmount = person.Income,
                NetGrowthRate = _calculatorOptions.SalaryNetGrowthRate,
                Ordinal = 0,
                RecurringAmount = (person.Income, FrequencyType.Yearly),
                OccurrenceType = OccurrenceType.EndOfPeriod,
                IsTaxable = true,
                TaxType = TaxType.Income
            };

            GenericCashFlowDefinition wealthCashFlowDefinition = new()
            {
                Id = Guid.Empty,
                Name = $"{person.Name} - Vermögen",
                InvestmentPeriod = (startingYear, numberOfPeriods),
                Flow = (AccountType.Exogenous, AccountType.Wealth),
                InitialAmount = person.Wealth,
                NetGrowthRate = _calculatorOptions.SalaryNetGrowthRate,
                Ordinal = 0,
                RecurringAmount = (decimal.Zero, FrequencyType.Yearly),
                OccurrenceType = OccurrenceType.BeginOfPeriod,
                IsTaxable = true,
                TaxType = TaxType.Wealth
            };

            return CalculateAsync(
                person, 
                new []{ salaryCashFlowDefinition, wealthCashFlowDefinition }.Concat(cashFlowDefinitions).ToList(),
                initialAccountValues);
        }
    }
}
