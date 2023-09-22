using System.Threading.Tasks;
using PensionCoach.Tools.CommonTypes.Tax;

namespace BlazorApp.Services
{
    public interface IMarginalTaxCurveCalculationService
    {
        Task<MarginalTaxResponse> CalculateIncomeCurveAsync(MarginalTaxRequest request);

        Task<int[]> SupportedTaxYearsAsync();
    }
}
