using System;
using System.Globalization;
using Application.Bvg;
using Xunit;
using Xunit.Abstractions;

namespace BvgCalculator.Tests;

[Trait("BVG", "Calculator")]
public class PensionSupplementCalculatorTests(ITestOutputHelper outputHelper)
{
    private readonly IPensionSupplementCalculator pensionSupplementCalculator = new BvgRevisionPensionSupplementCalculator();

    [Theory(DisplayName = "Rentenzuschlag")]
    [InlineData("1946-07-31", 0, 0)]
    [InlineData("1960-12-31", 0, 0)]
    [InlineData("1960-12-31", 220500, 0)]
    [InlineData("1961-01-01", 441000, 0)]
    [InlineData("1961-01-01", 100000, 2400)]
    [InlineData("1961-01-01", 500000, 0)]
    [InlineData("1969-01-01", 220500, 1800)]
    [InlineData("1969-01-01", 325239, 945)]
    [InlineData("1973-01-01", 220500, 1200)]
    [InlineData("1973-01-01", 325239, 630)]
    [InlineData("1977-01-01", 100000, 0)]
    public void Calculate_Pension_Supplement(
        string birthdateAsString, decimal finalRetirementCapitalTotal, decimal expectedPensionSupplement)
    {
        // given
        DateTime birthdate = DateTime.Parse(birthdateAsString, CultureInfo.InvariantCulture);

        // when
        decimal result = pensionSupplementCalculator.CalculatePensionSupplement(birthdate, finalRetirementCapitalTotal);

        outputHelper.WriteLine($"Calculated pension: {result}");

        // assert the result with expected pension supplement within a tolerance of 1
        Assert.Equal(expectedPensionSupplement, result, 1);
    }
}
