using System;
using Application.Bvg;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace BvgCalculator.Tests;

[Trait("BVG", "Retirement Date Calculator")]
public class BvgRetirementDateCalculatorTests(BvgRetirementDateCalculator fixture) : IClassFixture<BvgRetirementDateCalculator>
{
    [Theory(DisplayName = "Retirement Dates")]
    [InlineData(1, "1960-12-31", "2025-01-01")]
    [InlineData(2, "1961-01-01", "2026-02-01")]
    [InlineData(1, "1961-01-01", "2025-05-01")]
    [InlineData(2, "1962-01-01", "2027-02-01")]
    [InlineData(1, "1962-01-01", "2026-08-01")]
    [InlineData(2, "1963-01-01", "2028-02-01")]
    [InlineData(1, "1963-01-01", "2027-11-01")]
    [InlineData(2, "1969-03-17", "2034-04-01")]
    [InlineData(1, "1969-03-17", "2034-04-01")]
    [InlineData(1, "1955-03-17", "2019-04-01")]
    public void Return_Retirement_Date(int genderCode, string birthdateAsString, string expectedRetirementDateAsString)
    {
        // given
        DateTime expectedDateOfRetirement = DateTime.Parse(expectedRetirementDateAsString);
        DateTime birthdate = DateTime.Parse(birthdateAsString);
        Gender gender = (Gender)genderCode;

        // when
        DateTime result = fixture.DateOfRetirement(gender, birthdate);

        // then
        result.Should().Be(expectedDateOfRetirement);
    }
}
