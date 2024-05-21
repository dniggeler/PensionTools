using System;
using Application.Bvg.Models;
using Xunit;

namespace BvgCalculator.Tests;

[Trait("BVG Tests", "BVG Date")]
public class BvgDateTests
{
    [Fact(DisplayName = "BvgDate Constructor Sets Correct Values")]
    public void BvgDate_Constructor_Sets_Correct_Values()
    {
        // Arrange
        DateTime dateTime = new DateTime(2022, 1, 1);
        bool isEndOfDay = true;

        // Act
        BvgDate bvgDate = new BvgDate(dateTime, true);

        // Assert
        Assert.Equal(dateTime, bvgDate.DateTime);
        Assert.Equal(isEndOfDay, bvgDate.IsEndOfDay);
    }

    [Fact(DisplayName = "BvgDate ToDateTime Returns Correct Value")]
    public void BvgDate_ToDateTime_Returns_Correct_Value()
    {
        // Arrange
        DateTime dateTime = new DateTime(2022, 1, 1);
        BvgDate bvgDate = new BvgDate(dateTime, true);

        // Act
        DateTime result = bvgDate.ToDateTime();

        // Assert
        Assert.Equal(dateTime.AddDays(1), result);
    }

    [Fact(DisplayName = "BvgDate Implicit Conversion Returns Correct Value")]
    public void BvgDate_Implicit_Conversion_Returns_Correct_Value()
    {
        // Arrange
        DateTime dateTime = new DateTime(2022, 1, 1);

        // Act
        BvgDate bvgDate = dateTime;

        // Assert
        Assert.Equal(dateTime, bvgDate.DateTime);
        Assert.False(bvgDate.IsEndOfDay);
    }

    [Theory(DisplayName = "BvgDate Less Than Operator")]
    [InlineData("2022-01-01", "2022-01-01", true, true, false)]
    [InlineData("2022-01-01", "2022-01-01", true, false, false)]
    [InlineData("2022-01-01", "2022-01-01", false, true, true)]
    [InlineData("2022-01-01", "2022-01-02", true, false, true)]
    [InlineData("2022-01-01", "2022-01-02", false, true, true)]
    [InlineData("2022-01-02", "2022-01-01", true, false, false)]
    [InlineData("2022-01-02", "2022-01-01", false, true, false)]
    public void BvgDate_Less_Than_Operator_Return_Correct_Value(
        string leftDate, string rightDate, bool leftIsEndOfDay, bool rightIsEndOfDay, bool expected)
    {
        // Arrange
        DateTime leftDateTime = DateTime.Parse(leftDate);
        DateTime rightDateTime = DateTime.Parse(rightDate);
        BvgDate left = new BvgDate(leftDateTime, leftIsEndOfDay);
        BvgDate right = new BvgDate(rightDateTime, rightIsEndOfDay);

        // Act
        bool result = left < right;

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory(DisplayName = "BvgDate Less Or Equal Than Operator")]
    [InlineData("2022-01-01", "2022-01-01", true, true, true)]
    [InlineData("2022-01-01", "2022-01-01", true, false, false)]
    [InlineData("2022-01-01", "2022-01-01", false, true, true)]
    [InlineData("2022-01-01", "2022-01-01", false, false, true)]
    [InlineData("2021-12-31", "2022-01-01", true, false, true)]
    public void BvgDate_Less_Than_Or_Equal_Operator_Return_Correct_Value(
        string leftDate, string rightDate, bool leftIsEndOfDay, bool rightIsEndOfDay, bool expected)
    {
        // Arrange
        DateTime leftDateTime = DateTime.Parse(leftDate);
        DateTime rightDateTime = DateTime.Parse(rightDate);
        BvgDate left = new BvgDate(leftDateTime, leftIsEndOfDay);
        BvgDate right = new BvgDate(rightDateTime, rightIsEndOfDay);

        // Act
        bool result = left <= right;

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory(DisplayName = "BvgDate Equal Operator")]
    [InlineData("2022-01-01", "2022-01-01", true, true, true)]
    [InlineData("2022-01-01", "2022-01-01", false, false, true)]
    [InlineData("2021-12-31", "2022-01-01", true, false, false)]
    public void BvgDate_Equal_Operator_Return_Correct_Value(
        string leftDate, string rightDate, bool leftIsEndOfDay, bool rightIsEndOfDay, bool expected)
    {
        // Arrange
        DateTime leftDateTime = DateTime.Parse(leftDate);
        DateTime rightDateTime = DateTime.Parse(rightDate);
        BvgDate left = new BvgDate(leftDateTime, leftIsEndOfDay);
        BvgDate right = new BvgDate(rightDateTime, rightIsEndOfDay);

        // Act
        bool result = left == right;

        // Assert
        Assert.Equal(expected, result);
    }
}
