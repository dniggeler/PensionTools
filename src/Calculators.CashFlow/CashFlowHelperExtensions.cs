using System;
using System.Collections.Generic;
using System.Linq;
using Calculators.CashFlow.Models;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;
using PensionCoach.Tools.CommonTypes.Tax;

namespace Calculators.CashFlow
{
    public static class CashFlowHelperExtensions
    {
        public static IEnumerable<CashFlowModel> GenerateCashFlow(this GenericCashFlowDefinition definition)
        {
            var range = Enumerable.Range(
                definition.InvestmentPeriod.Year,
                definition.InvestmentPeriod.NumberOfPeriods);

            yield return new CashFlowModel(
                new DateOnly(definition.InvestmentPeriod.Year, 1, 1),
                definition.InitialAmount,
                AccountType.Exogenous,
                definition.Flow.Target,
                definition.IsTaxable,
                definition.TaxType,
                definition.OccurrenceType);

            decimal cashFlow = definition.RecurringInvestment.Amount;

            foreach (var year in range)
            {
                yield return new CashFlowModel(
                    new DateOnly(year, 1, 1),
                    cashFlow,
                    definition.Flow.Source,
                    definition.Flow.Target,
                    definition.IsTaxable,
                    definition.TaxType,
                    definition.OccurrenceType);

                cashFlow *= decimal.One + definition.NetGrowthRate;
            }
        }

        public static IEnumerable<CashFlowModel> GenerateCashFlow(this ThirdPillarCashFlowDefinition thirdPillarDefinition)
        {
            var definition = new GenericCashFlowDefinition
            {
                Header = thirdPillarDefinition.Header,
                DateOfStart = thirdPillarDefinition.DateOfStart,
                InitialAmount = thirdPillarDefinition.InitialAmount,
                NetGrowthRate = thirdPillarDefinition.NetGrowthRate,
                Flow = new FlowPair(AccountType.Income, AccountType.CapitalBenefits),
                RecurringInvestment = new RecurringInvestment
                {
                    Amount = thirdPillarDefinition.YearlyInvestmentAmount,
                    Frequency = FrequencyType.Yearly,
                },
                InvestmentPeriod = new InvestmentPeriod
                {
                    Year = thirdPillarDefinition.DateOfStart.Year,
                    NumberOfPeriods = thirdPillarDefinition.NumberOfInvestments,
                },
                IsTaxable = false,
                TaxType = TaxType.Undefined,
                OccurrenceType = OccurrenceType.BeginOfPeriod
            };

            foreach (var cashFlowModel in definition.GenerateCashFlow())
            {
                yield return cashFlowModel;
            }

            var range = Enumerable.Range(
                definition.InvestmentPeriod.Year,
                definition.InvestmentPeriod.NumberOfPeriods);

            yield return new CashFlowModel(
                new DateOnly(definition.InvestmentPeriod.Year, 1, 1),
                definition.InitialAmount,
                AccountType.Exogenous,
                definition.Flow.Target,
                definition.IsTaxable,
                definition.TaxType,
                definition.OccurrenceType);

            decimal cashFlow = definition.RecurringInvestment.Amount;
        }

        public static IEnumerable<CashFlowModel> AggregateCashFlows(this IEnumerable<CashFlowModel> cashFlows)
        {
            return cashFlows
                .GroupBy(keySelector => new
                {
                    keySelector.DateOfOccurrence,
                    keySelector.Source,
                    keySelector.Target,
                    keySelector.IsTaxable,
                    keySelector.TaxType,
                    keySelector.OccurrenceType
                })
                .Select(g => new CashFlowModel(
                    g.Key.DateOfOccurrence,
                    g.Sum(item => item.Amount),
                    g.Key.Source,
                    g.Key.Target,
                    g.Key.IsTaxable,
                    g.Key.TaxType,
                    g.Key.OccurrenceType));
        }
    }
}
