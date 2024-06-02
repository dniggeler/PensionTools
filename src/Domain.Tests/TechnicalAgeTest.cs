using Domain.Models.Bvg;
using FluentAssertions;

namespace Domain.Tests;

[Trait("Domain Tests", "Technical Age")]
public class TechnicalAgeTest
{
    [Theory(DisplayName = "operator +")]
    [InlineData(33, 1, 33, 1, 66, 2)]
    [InlineData(58, 0, 1, 0, 59, 0)]
    [InlineData(58, 6, 1, 8, 60, 2)]
    public void Add(int year1, int month1, int year2, int month2, int expectedYear, int expectedMonth)
    {
        TechnicalAge age1 = new(year1, month1);
        TechnicalAge age2 = new(year2, month2);

        TechnicalAge expectedAge = new(expectedYear, expectedMonth);

        TechnicalAge result = age1 + age2;

        result.Should().BeEquivalentTo(expectedAge);
    }

    [Theory(DisplayName = "operator -")]
    [InlineData(33, 1, 33, 1, 0, 0)]
    [InlineData(65, 0, 58, 6, 6, 6)]
    [InlineData(64, 8, 58, 6, 6, 2)]
    [InlineData(64, 8, 58, 9, 5, 11)]
    public void Subtract(int year1, int month1, int year2, int month2, int expectedYear, int expectedMonth)
    {
        TechnicalAge age1 = new(year1, month1);
        TechnicalAge age2 = new(year2, month2);

        TechnicalAge expectedAge = new(expectedYear, expectedMonth);

        TechnicalAge result = age1 - age2;

        result.Should().BeEquivalentTo(expectedAge);
    }

    [Theory(DisplayName = "operator <=")]
    [InlineData(33, 1, 33, 1, true)]
    [InlineData(65, 0, 58, 6, false)]
    [InlineData(64, 8, 58, 6, false)]
    [InlineData(58, 8, 58, 9, true)]
    public void LessOrEqual(int year1, int month1, int year2, int month2, bool expected)
    {
        TechnicalAge age1 = new(year1, month1);
        TechnicalAge age2 = new(year2, month2);

        bool result = age1 <= age2;

        result.Should().Be(expected);
    }

    [Theory(DisplayName = "operator >=")]
    [InlineData(33, 1, 33, 1, true)]
    [InlineData(65, 0, 58, 6, true)]
    [InlineData(64, 8, 58, 6, true)]
    [InlineData(58, 8, 58, 9, false)]
    public void GreaterOrEqual(int year1, int month1, int year2, int month2, bool expected)
    {
        TechnicalAge age1 = new(year1, month1);
        TechnicalAge age2 = new(year2, month2);

        bool result = age1 >= age2;

        result.Should().Be(expected);
    }
}
