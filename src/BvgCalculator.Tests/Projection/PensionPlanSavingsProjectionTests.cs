using System;
using System.Globalization;
using System.Linq;
using Application.Bvg;
using Application.Bvg.Models;
using Domain.Enums;
using Domain.Models.Bvg;
using Xunit;
using Xunit.Abstractions;

namespace BvgCalculator.Tests.Projection;

[Trait("Savings Process Projection Calculator", "Pension Plan")]
public class PensionPlanSavingsProjectionTests(ITestOutputHelper outputHelper)
{
    const decimal Deviation = 10;

    private readonly SingleSavingsProcessProjectionCalculator calculator = new();
    private readonly BvgRetirementDateCalculator retirementDateCalculator = new();

    [Theory]
    [InlineData("1968-06-12", 1, 5090, 0.02, 2025, 75642.55, 97914)]
    [InlineData("1969-03-17", 2, 192967, 0.01, 2024, 721065.05, 1171484)]
    public void ProjectionTableTheory(
        string dateOfBirthAsString,
        int genderCode,
        decimal insuredSalary,
        decimal projectionInterestRate,
        int yearOfBeginProjection,
        decimal beginOfRetirementCapital,
        decimal expectedFinalRetirementCapital)
    {
        // Arrange
        Gender gender = (Gender)genderCode;
        DateTime dateOfBirth = DateTime.Parse(dateOfBirthAsString, CultureInfo.InvariantCulture);
        DateTime dateOfRetirement = retirementDateCalculator.DateOfRetirement(gender, dateOfBirth);
        TechnicalAge retirementAge = retirementDateCalculator.RetirementAge(gender, dateOfBirth);
        TechnicalAge finalAge = retirementAge;

        // Act
        RetirementSavingsProcessResult[] projections = calculator.ProjectionTable(
            projectionInterestRate,
            dateOfRetirement,
            dateOfRetirement,
            retirementAge,
            finalAge,
            yearOfBeginProjection,
            beginOfRetirementCapital,
            RetirementCreditSelector(insuredSalary));

        decimal? result = projections.SingleOrDefault(item => item.IsRetirementDate)?.RetirementCapital;

        // Assert
        Assert.NotNull(result);
        Assert.InRange(result.Value, expectedFinalRetirementCapital - Deviation, expectedFinalRetirementCapital + Deviation);
        outputHelper.WriteLine($"Expected vs Actual: {expectedFinalRetirementCapital}, Actual: {result}");
    }

    private Func<TechnicalAge, decimal> RetirementCreditSelector(decimal insuredSalary)
    {
        decimal retirementCreditFactor = 0.18M;
        return technicalAge =>
        {
            return technicalAge switch
            {
                _ when technicalAge <= (65, 0) => insuredSalary * retirementCreditFactor,
                _ => 0
            };
        };
    }
}
