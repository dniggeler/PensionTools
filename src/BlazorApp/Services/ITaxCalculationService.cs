using System.Threading.Tasks;
using PensionCoach.Tools.CommonTypes.Tax;

namespace BlazorApp.Services
{
    public interface ITaxCalculationService
    {
        Task<FullTaxResponse> CalculateAsync(FullTaxRequest request);

        Task<CapitalBenefitTaxResponse> CalculateAsync(CapitalBenefitTaxRequest request);

        Task<int[]> SupportedTaxYearsAsync();
    }
}
