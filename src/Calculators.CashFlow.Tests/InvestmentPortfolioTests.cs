using System;
using System.Collections.Generic;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;
using Snapshooter.Xunit;
using Xunit;

namespace Calculators.CashFlow.Tests;

[Trait("Higher Level Calculators", "Cash-Flow")]
public class InvestmentPortfolioTests
{
    [Fact(DisplayName = "Portfolio Growth")]
    public void Calculate_Growth_Of_Investment_Portfolio()
    {
        // given
        InvestmentPortfolioDefinition definition = new ()
        {
            Header = new CashFlowHeader
            {
                Id = "test",
                Name = "Test",
            },
            DateOfProcess = new DateTime(2021, 1, 1),
            NetCapitalGrowthRate = 0.02M,
            NetInterestRate = 0.01M,
            InitialInvestment = 100_000,
            RecurringInvestment = new RecurringInvestment
            {
                Amount = 6723,
                Frequency = FrequencyType.Yearly,
            },
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = 2021,
                NumberOfPeriods = 10
            },
        };

        // when
        IEnumerable<ICashFlowDefinition> result = definition.CreateGenericDefinition();

        // then
        Snapshot.Match(result);
    }
}
