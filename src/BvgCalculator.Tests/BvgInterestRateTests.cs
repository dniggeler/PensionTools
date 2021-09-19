using FluentAssertions;
using PensionCoach.Tools.BvgCalculator;
using Xunit;

namespace BvgCalculator.Tests
{
    [Trait("BVG", "Interest Rate")]
    public class BvgInterestRateTests
    {
        [Theory(DisplayName = "Retirement Credit Rate")]
        [InlineData(2015, 0.0175)]
        [InlineData(1970, 0)]
        [InlineData(1985, 0.04)]
        [InlineData(2017, 0.01)]
        public void ShouldReturnRetirementCreditRate(int year, decimal expectedRate)
        {
            // given

            // when
            decimal result = Bvg.GetInterestRate(year);

            // then
            result.Should().Be(expectedRate);
        }
    }
}
