using System.Collections.Generic;
using System.Linq;
using Calculators.CashFlow.Models;

namespace Calculators.CashFlow
{
    public class CashFlowGenerator
    {
        public IEnumerable<CashFlowModel> Generate(GenericCashFlowDefinition definition)
        {
            var range = Enumerable.Range(
                definition.InvestmentPeriod.BeginYear + 1,
                definition.InvestmentPeriod.Count -1 );

            yield return new CashFlowModel(definition.InvestmentPeriod.BeginYear, definition.InitialAmount, definition.Flow.Source, definition.Flow.Target);

            decimal cashFlow = definition.RecurringAmount.Amount;

            foreach (var year in range)
            {
                cashFlow *= decimal.One + definition.NetGrowthRate;

                yield return new CashFlowModel(year, cashFlow, definition.Flow.Source, definition.Flow.Target);
            }
        }
    }
}
