using Application.Bvg;
using FluentAssertions;
using Xunit;

namespace BvgCalculator.Tests
{
    [Trait("BVG", "Retirement Credit Table")]
    public class BvgRetirementCreditTableTests : IClassFixture<BvgCalculatorFixture>
    {
        [Theory(DisplayName = "Retirement Credit Rate")]
        [InlineData(30, 0.07)]
        [InlineData(65, 0.18)]
        [InlineData(35, 0.10)]
        [InlineData(24, 0.0)]
        public void ShouldReturnRetirementCreditRate(int age, decimal expectedRate)
        {
            // given

            // when
            BvgRetirementCreditsTable creditTable = new BvgRetirementCreditsTable();
            decimal result = creditTable.GetRateInPercentage(age);

            // then
            result.Should().Be(expectedRate);
        }
    }
}
