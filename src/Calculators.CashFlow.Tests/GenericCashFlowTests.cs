using System.Collections.Generic;
using System.Linq;
using Calculators.CashFlow.Models;
using Snapshooter.Xunit;
using Xunit;

namespace Calculators.CashFlow.Tests
{
    [Trait("Higher Level Calculators", "Cash-Flow")]
    public class GenericCashFlowTests
    {
        [Fact(DisplayName = "Single Allocation for 10 Years")]
        public void Generate_Single_Allocation_For_10_Years()
        {
            // given
            GenericCashFlowDefinition definition = new GenericCashFlowDefinition
            {
                NetGrowthRate = 0,
                Name = "Test",
                InitialAmount = 10_000,
                RecurringAmount = (500, FrequencyType.Yearly),
                Flow = (FundsType.Exogenous, FundsType.TaxableWealth),
                InvestmentPeriod = (2021, 10)
            };

            CashFlowGenerator generator = new CashFlowGenerator();

            // when
            List<CashFlowModel> result = generator.Generate(definition).ToList();

            // then
            Snapshot.Match(result);
        }
    }
}
