using System;
using System.Collections.Generic;
using System.Linq;
using Application.Features.ContributionCalculator;
using Application.Features.ContributionCalculator.Models;
using FluentAssertions;
using Snapshooter.Xunit;
using Xunit;

namespace Calculators.CashFlow.Tests;

public class CompoundingContributionCalculatorTests
{
    private readonly CompoundingContributionCalculator _calculator = new();

    [Fact]
    public void Calculate_WithSingleInitialAmount_ShouldAccrueAnnualInterestCorrectly()
    {
        // Arrange
        var startDate = new DateTime(2020, 1, 1);
        var finalDate = new DateTime(2021, 1, 1); // 1 year exactly
        var initialAmount = 1000m;
        var rate = 0.05m; // 5%

        var request = new ContributionRequest(
            startDate,
            initialAmount,
            [],
            rate,
            finalDate,
            CompoundingFrequency.Annual,
            ReturnSequence: false);

        // Act
        var result = _calculator.Calculate(request);

        // Assert
        result.IsRight.Should().BeTrue();
        result.IfRight(r =>
        {
            r.FinalAmount.Should().BeApproximately(1050m, 0.01m);
            r.Sequence.Should().BeNull();
        });
    }

    [Fact]
    public void Calculate_WithTenContributions_ShouldHandleCorrectly()
    {
        // Arrange
        var startDate = new DateTime(2020, 1, 1);
        var finalDate = new DateTime(2030, 1, 1);
        var initialAmount = 100000m;
        var rate = 0.025m; // 2.5%

        // 10 monthly contributions from Feb to Nov
        var contributionAmount = 5000m;
        var contributions = new List<Contribution>();
        for (int year = 1; year < 11; year++)
        {
            contributions.Add(new Contribution(new DateTime(2020+year, 1, 1), contributionAmount));
        }

        var request = new ContributionRequest(
            startDate,
            initialAmount,
            contributions,
            rate,
            finalDate,
            CompoundingFrequency.Annual,
            ReturnSequence: true);

        // Act
        var result = _calculator.Calculate(request);

        // Assert
        result.IsRight.Should().BeTrue();
        Snapshot.Match(result);
    }

    [Fact]
    public void Calculate_WithContributions_ShouldAccumulateCorrectly()
    {
        // Arrange
        var startDate = new DateTime(2020, 1, 1);
        var midDate = new DateTime(2020, 7, 1); // 6 months later
        var finalDate = new DateTime(2021, 1, 1);
        var initialAmount = 1000m;
        var contributionAmount = 500m;
        var rate = 0.10m;

        // 1. Initial 1000 grows for 6 months (0.5 years) -> 1000 * (1.1)^0.5 = 1048.81 approx
        // 2. Add 500 -> 1548.81
        // 3. Grow for another 6 months -> 1548.81 * (1.1)^0.5 = 1624.40 approx
            
        // Wait, standard annual compounding for fractional years is (1+r)^t.
        // 1000 * (1.1)^1 + 500 * (1.1)^0.5? No, it's sequential.
        // Period 1: 1000 * (1.1)^0.5 = 1048.8088
        // Balance: 1548.8088
        // Period 2: 1548.8088 * (1.1)^0.5 = 1624.40
            
        // Direct calculation:
        // FV = 1000 * (1.1)^1 + 500 * (1.1)^0.5
        // 1000 * 1.1 = 1100
        // 500 * 1.0488... = 524.40
        // Sum = 1624.40
            
        var contributions = new List<Contribution>
        {
            new Contribution(midDate, contributionAmount)
        };

        var request = new ContributionRequest(
            startDate,
            initialAmount,
            contributions,
            rate,
            finalDate,
            CompoundingFrequency.Annual,
            ReturnSequence: true);

        // Act
        var result = _calculator.Calculate(request);

        // Assert
        result.IsRight.Should().BeTrue();
        result.IfRight(r =>
        {
            r.FinalAmount.Should().BeApproximately(1624.40m, 0.05m);
            r.Sequence.Should().NotBeNull();
            r.Sequence.Should().HaveCount(3); // Initial, Contribution, Final
        });
    }

    [Fact]
    public void Calculate_WithMonthlyCompounding_ShouldBeDifferent()
    {
        // Arrange
        var startDate = new DateTime(2020, 1, 1);
        var finalDate = new DateTime(2021, 1, 1);
        var initialAmount = 1000m;
        var rate = 0.12m; // 12% annual -> 1% monthly

        var request = new ContributionRequest(
            startDate,
            initialAmount,
            Enumerable.Empty<Contribution>(),
            rate,
            finalDate,
            CompoundingFrequency.Monthly,
            ReturnSequence: false);

        // Act
        var result = _calculator.Calculate(request);

        // Assert
        result.IsRight.Should().BeTrue();
        result.IfRight(r =>
        {
            // 1000 * (1.01)^12 = 1126.825
            r.FinalAmount.Should().BeApproximately(1126.83m, 0.01m);
        });
    }
}
