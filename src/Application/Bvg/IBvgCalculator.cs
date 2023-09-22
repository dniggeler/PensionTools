using Application.Bvg.Models;
using LanguageExt;

namespace Application.Bvg;

public interface IBvgCalculator
{
    Task<Either<string, BvgCalculationResult>> CalculateAsync(
        PredecessorRetirementCapital predecessorCapital, DateTime dateOfProcess, BvgPerson person);
}
