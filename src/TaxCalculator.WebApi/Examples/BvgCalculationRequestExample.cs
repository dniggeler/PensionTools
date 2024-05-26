using System;
using Domain.Enums;
using PensionCoach.Tools.CommonTypes.Bvg;
using Swashbuckle.AspNetCore.Filters;

namespace TaxCalculator.WebApi.Examples;

public class BvgCalculationRequestExample : IExamplesProvider<BvgCalculationRequest>
{
    public BvgCalculationRequest GetExamples()
    {
        return new BvgCalculationRequest
        {
            Name = "Test",
            DateOfBirth = new DateTime(2000, 4, 19),
            CalculationYear = 2019,
            Gender = Gender.Male,
            Salary = 100_000,
            RetirementCapitalEndOfYear = 0,
        };
    }
}
