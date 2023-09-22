using System.Threading.Tasks;
using Application.Features.FullTaxCalculation;
using PensionCoach.Tools.CommonTypes.Tax;

namespace BlazorApp.Services
{
    public interface ITaxCalculationService
    {
        Task<FullTaxResponse> CalculateAsync(FullTaxRequest request);

        Task<int[]> SupportedTaxYearsAsync();
    }
}
