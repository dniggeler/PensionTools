using System.Collections.Generic;
using System.Linq;
using Calculators.CashFlow.Models;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.Tax;
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
                RecurringInvestment = new RecurringInvestment(500, FrequencyType.Yearly),
                Flow = new FlowPair(AccountType.Exogenous, AccountType.Wealth),
                InvestmentPeriod = new InvestmentPeriod(2021, 10),
                IsTaxable = true,
                TaxType = TaxType.Wealth
            };

            // when
            List<CashFlowModel> result = definition.GenerateCashFlow().ToList();

            // then
            Snapshot.Match(result);
        }

        [Fact(DisplayName = "Aggregate Multiple Cash-Flows")]
        public void Aggregate_Multiple_CashFlows()
        {
            // given
            GenericCashFlowDefinition definition1 = new GenericCashFlowDefinition
            {
                NetGrowthRate = 0,
                Name = "Test",
                InitialAmount = 10_000,
                RecurringInvestment = new RecurringInvestment(500, FrequencyType.Yearly),
                Flow = new FlowPair(AccountType.Exogenous, AccountType.Wealth),
                InvestmentPeriod = new InvestmentPeriod(2021, 10),
                IsTaxable = true,
                TaxType = TaxType.Wealth
            };

            GenericCashFlowDefinition definition2 = new GenericCashFlowDefinition
            {
                NetGrowthRate = 0,
                Name = "Test 2",
                InitialAmount = 20_000,
                RecurringInvestment = new RecurringInvestment(500, FrequencyType.Yearly),
                Flow = new FlowPair(AccountType.Exogenous, AccountType.Wealth),
                InvestmentPeriod = new InvestmentPeriod(2021, 5),
                IsTaxable = true,
                TaxType = TaxType.Wealth
            };

            GenericCashFlowDefinition definition3 = new GenericCashFlowDefinition
            {
                NetGrowthRate = 0,
                Name = "Test 3",
                InitialAmount = 50_000,
                RecurringInvestment = new RecurringInvestment(0, FrequencyType.Yearly),
                Flow = new FlowPair(AccountType.Income, AccountType.CapitalBenefits),
                InvestmentPeriod = new InvestmentPeriod(2021, 1),
            };

            // when
            IEnumerable<CashFlowModel> result = new[] { definition1, definition2, definition3 }
                .SelectMany(d => d.GenerateCashFlow())
                .AggregateCashFlows();

            // then
            Snapshot.Match(result);
        }
    }
}
