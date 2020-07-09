using System;
using System.Threading.Tasks;
using PensionCoach.Tools.BvgCalculator.Models;

namespace PensionCoach.Tools.BvgCalculator
{
    public interface IBvgCalculator
    {
        Task<BvgCalculationResult> CalculateAsync(
            BvgProcessActuarialReserve predecessor, DateTime dateOfProcess, BvgPerson person);
    }
}
