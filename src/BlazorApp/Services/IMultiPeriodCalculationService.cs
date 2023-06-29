using System.Threading.Tasks;
using PensionCoach.Tools.CommonTypes.MultiPeriod;

namespace BlazorApp.Services;

public interface IMultiPeriodCalculationService
{
    Task<MultiPeriodResponse> CalculateAsync(MultiPeriodRequest request);
}
