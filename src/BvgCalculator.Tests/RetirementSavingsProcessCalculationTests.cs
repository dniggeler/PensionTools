using System;
using Application.Bvg;
using Application.Bvg.Models;
using Application.Extensions;
using Domain.Models.Bvg;
using Snapshooter.Xunit;
using Xunit;

namespace BvgCalculator.Tests;

[Trait("Savings Process Projection Calculator", "BVG Revision")]
public class RetirementSavingsProcessCalculationTests
{
    const decimal ProjectionInterestRate = 0.0125m;
    readonly DateTime startOfBvgRevision = new(2026, 1, 1);

    [Fact(DisplayName = "Single Savings Process Projection Table")]
    public void SavingsProcessTable_WithSampleInputs_ReturnsExpectedResults()
    {
        // Arrange
        DateTime processDate = new(2024, 1, 1);
        DateTime dateOfBirth = new(1969, 3, 17);

        DateTime dateOfRetirement = new(2034, 4, 1);
        TechnicalAge retirementAge = (65, 0);
        TechnicalAge finalAge = (70, 0);
        int yearOfBeginSavingsProcess = 2024;
        decimal beginOfRetirementCapital = 0;

        ISavingsProcessProjectionCalculator calculator = new SingleSavingsProcessProjectionCalculator();

        // Act
        RetirementSavingsProcessResult[] actualResult = calculator.ProjectionTable(
            ProjectionInterestRate,
            dateOfRetirement,
            dateOfRetirement,
            retirementAge,
            finalAge,
            yearOfBeginSavingsProcess,
            beginOfRetirementCapital,
            GetRetirementCredit(dateOfBirth));

        // Assert
        Assert.NotNull(actualResult);
        Snapshot.Match(actualResult);
    }

    private Func<TechnicalAge, decimal> GetRetirementCredit(DateTime dateOfBirth)
    {
        TimeSpan diff = startOfBvgRevision.Subtract(dateOfBirth.GetBirthdateTechnical());

        // calculate the number of years and months from diff
        int years = diff.Days / 365;
        int months = (diff.Days % 365) / 30;

        return technicalAge =>
        {
            decimal beforeRevision = 11245.5M;
            decimal revision = 9878.4M;

            return technicalAge <= (years, months) ? beforeRevision : revision;
        };
    }
}
