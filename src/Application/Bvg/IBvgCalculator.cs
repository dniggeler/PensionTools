using Application.Bvg.Models;
using Domain.Models.Bvg;
using LanguageExt;

namespace Application.Bvg;

public interface IBvgCalculator
{
    Task<Either<string, BvgCalculationResult>> CalculateAsync(PredecessorRetirementCapital predecessorCapital, DateTime dateOfProcess, BvgPerson person);

    Either<string, decimal> InsuredSalary(DateTime dateOfProcess, BvgPerson person);

    Either<string, BvgDataPoint[]> InsuredSalaries(DateTime dateOfProcess, BvgPerson person);
}
