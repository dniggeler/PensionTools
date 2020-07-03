using System;
using System.Threading.Tasks;
using PensionCoach.Tools.BvgCalculator.Models;

namespace PensionCoach.Tools.BvgCalculator
{
    public interface IBvgCalculator
    {
        Task<BvgCalculationResult> CalculateAsync(DateTime dateOfProcess, BvgPerson person);
    }
}
