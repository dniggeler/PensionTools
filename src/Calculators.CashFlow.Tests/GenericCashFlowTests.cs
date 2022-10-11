using System;
using System.Collections.Generic;
using System.Linq;
using Calculators.CashFlow.Models;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;
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
                Header = new CashFlowHeader
                {
                    Id = "test",
                    Name = "Test",
                },
                DateOfStart = new DateTime(2021, 1, 1),
                NetGrowthRate = 0,
                InitialAmount = 9_500,
                RecurringInvestment = new RecurringInvestment
                {
                    Amount = 500,
                    Frequency = FrequencyType.Yearly,
                },
                Flow = new FlowPair(AccountType.Exogenous, AccountType.Wealth),
                InvestmentPeriod = new InvestmentPeriod
                {
                    Year = 2021,
                    NumberOfPeriods = 10
                },
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
                Header = new CashFlowHeader
                {
                    Id = "test1",
                    Name = "Test",
                },
                NetGrowthRate = 0,
                InitialAmount = 9_500,
                RecurringInvestment = new RecurringInvestment
                {
                    Amount = 500,
                    Frequency = FrequencyType.Yearly,
                },
                Flow = new FlowPair(AccountType.Exogenous, AccountType.Wealth),
                InvestmentPeriod = new InvestmentPeriod
                {
                    Year = 2021,
                    NumberOfPeriods = 10
                },
                IsTaxable = true,
                TaxType = TaxType.Wealth
            };

            GenericCashFlowDefinition definition2 = new GenericCashFlowDefinition
            {
                Header = new CashFlowHeader
                {
                    Id = "test2",
                    Name = "Test 2",
                },
                NetGrowthRate = 0,
                InitialAmount = 19_500,
                RecurringInvestment = new RecurringInvestment
                {
                    Amount = 500,
                    Frequency = FrequencyType.Yearly,
                },
                Flow = new FlowPair(AccountType.Exogenous, AccountType.Wealth),
                InvestmentPeriod = new InvestmentPeriod
                {
                    Year = 2021,
                    NumberOfPeriods = 5
                },
                IsTaxable = true,
                TaxType = TaxType.Wealth
            };

            GenericCashFlowDefinition definition3 = new GenericCashFlowDefinition
            {
                Header = new CashFlowHeader
                {
                    Id = "test3",
                    Name = "Test 3",
                },
                NetGrowthRate = 0,
                InitialAmount = 50_000,
                RecurringInvestment = new RecurringInvestment
                {
                    Amount = 0,
                    Frequency = FrequencyType.Yearly,
                },
                Flow = new FlowPair(AccountType.Income, AccountType.CapitalBenefits),
                InvestmentPeriod = new InvestmentPeriod
                {
                    Year = 2021,
                    NumberOfPeriods = 0
                },
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
