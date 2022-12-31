using System.Threading.Tasks;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.TaxComparison;

namespace BlazorApp.Services
{
    public interface ITaxScenarioService
    {
        Task<CapitalBenefitsTransferInResponse> CalculateAsync(CapitalBenefitTransferInComparerRequest request);
    }
}
