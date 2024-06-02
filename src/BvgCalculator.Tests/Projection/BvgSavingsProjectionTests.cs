using System;
using Application.Bvg;
using Application.Extensions;
using Domain.Models.Bvg;
using Snapshooter.Xunit;
using Xunit;

namespace BvgCalculator.Tests.Projection;

[Trait("Savings Process Projection Calculator", "BVG Revision")]
public class BvgSavingsProjectionTests
{
    const decimal ProjectionInterestRate = 0.0125m;
    readonly DateTime startOfBvgRevision = new(2026, 1, 1);

    [Fact(DisplayName = "Single Savings Process Projection Table")]
    public void SavingsProcessTable_WithSampleInputs_ReturnsExpectedResults()
    {
        // Arrange
        DateTime dateOfBirth = new(1969, 3, 17);

        DateTime dateOfRetirement = new(2034, 4, 1);
        TechnicalAge retirementAge = (65, 0);
        TechnicalAge finalAge = (65, 0);
        var yearOfBeginSavingsProcess = 2025;
        decimal beginOfRetirementCapital = 11245.5M;

        ISavingsProcessProjectionCalculator calculator = new SingleSavingsProcessProjectionCalculator();

        // Act
        var actualResult = calculator.ProjectionTable(
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
        var diff = startOfBvgRevision.Subtract(dateOfBirth.GetBirthdateTechnical());

        // calculate the number of years and months from diff
        var years = diff.Days / 365;
        var months = diff.Days % 365 / 30;

        return technicalAge =>
        {
            var beforeRevision = 11245.5M;
            var revision = 9878.4M;

            return technicalAge <= (years, months) ? beforeRevision : revision;
        };
    }
}
