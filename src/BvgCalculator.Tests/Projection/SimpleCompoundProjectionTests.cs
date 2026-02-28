using System;
using Application.Bvg;
using Domain.Models.Bvg;
using Snapshooter.Xunit;
using Xunit;

namespace BvgCalculator.Tests.Projection;

[Trait("Savings Process Projection Calculator", "Simple Interest Compounding")]
public class SimpleCompoundProjectionTests
{
    const decimal ProjectionInterestRate = 0.0125m;

    [Fact(DisplayName = "Compounding with no periodic investment")]
    public void Compounding_With_No_Periodic_Investment()
    {
        // Arrange
        DateTime dateOfBirth = new(1969, 3, 17);

        DateTime dateOfRetirement = new(2034, 4, 1);
        TechnicalAge retirementAge = (65, 0);
        TechnicalAge finalAge = (65, 0);
        var yearOfBeginSavingsProcess = 2026;
        decimal beginOfRetirementCapital = 100000;

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
            NoInvestment());

        // Assert
        Assert.NotNull(actualResult);
        Snapshot.Match(actualResult);
    }

    private Func<TechnicalAge, decimal> NoInvestment()
    {
        return _ => decimal.Zero;
    }
}
