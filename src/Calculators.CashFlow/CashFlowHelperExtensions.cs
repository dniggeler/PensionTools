using System.Collections.Generic;
using System.Linq;
using Calculators.CashFlow.Models;

namespace Calculators.CashFlow
{
    public static class CashFlowHelperExtensions
    {
        public static IEnumerable<CashFlowModel> GenerateCashFlow(this GenericCashFlowDefinition definition)
        {
            var range = Enumerable.Range(
                definition.InvestmentPeriod.BeginYear + 1,
                definition.InvestmentPeriod.Count - 1);

            yield return new CashFlowModel(
                definition.InvestmentPeriod.BeginYear,
                definition.InitialAmount,
                definition.Flow.Source,
                definition.Flow.Target,
                definition.IsTaxable,
                definition.TaxType,
                definition.OccurrenceType);

            decimal cashFlow = definition.RecurringAmount.Amount;

            foreach (var year in range)
            {
                cashFlow *= decimal.One + definition.NetGrowthRate;

                yield return new CashFlowModel(
                    year,
                    cashFlow,
                    definition.Flow.Source,
                    definition.Flow.Target,
                    definition.IsTaxable,
                    definition.TaxType,
                    definition.OccurrenceType);
            }
        }

        public static IEnumerable<CashFlowModel> AggregateCashFlows(this IEnumerable<CashFlowModel> cashFlows)
        {
            return cashFlows
                .GroupBy(keySelector => new
                {
                    keySelector.Year,
                    keySelector.Source,
                    keySelector.Target,
                    keySelector.IsTaxable,
                    keySelector.TaxType,
                    keySelector.OccurrenceType
                })
                .Select(g => new CashFlowModel(
                    g.Key.Year,
                    g.Sum(item => item.Amount),
                    g.Key.Source,
                    g.Key.Target,
                    g.Key.IsTaxable,
                    g.Key.TaxType,
                    g.Key.OccurrenceType));
        }
    }
}
