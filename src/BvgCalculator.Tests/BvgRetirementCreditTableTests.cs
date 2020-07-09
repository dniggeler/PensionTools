using PensionCoach.Tools.BvgCalculator;
using Xunit;

namespace BvgCalculator.Tests
{
    [Trait("BVG", "Retirement Credit Table")]
    public class BvgRetirementCreditTableTests : IClassFixture<BvgCalculatorFixture>
    {
        [Fact(DisplayName = "Retirement Credit Rate")]
        public void ShouldReturnRetirementCreditRate()
        {
            // given
            int bvgAge = 30;
            decimal expectedResult = 7M;

            // when
            BvgRetirementCreditsTable creditTable = new BvgRetirementCreditsTable();
            decimal result = creditTable.GetRateInPercentage(bvgAge);

            // then
            result.Should().Be(expectedResult);
        }
    }
}
