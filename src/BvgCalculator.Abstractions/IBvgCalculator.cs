using System;
using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.BvgCalculator.Models;

namespace PensionCoach.Tools.BvgCalculator
{
    public interface IBvgCalculator
    {
        Task<Either<string, BvgCalculationResult>> CalculateAsync(
            PredecessorRetirementCapital predecessorCapital, DateTime dateOfProcess, BvgPerson person);
    }
}
