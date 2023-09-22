using System.Threading.Tasks;
using Application.Features.TaxComparison.Models;
using PensionCoach.Tools.CommonTypes.MultiPeriod;

namespace BlazorApp.Services
{
    public interface ITaxScenarioService
    {
        Task<CapitalBenefitsTransferInResponse> CalculateAsync(CapitalBenefitTransferInComparerRequest request);
    }
}
