using System.Threading.Tasks;
using Application.Features.TaxScenarios.Models;
using PensionCoach.Tools.CommonTypes.Features.PensionVersusCapital;
using PensionCoach.Tools.CommonTypes.MultiPeriod;

namespace BlazorApp.Services;

public interface ITaxScenarioService
{
    Task<CapitalBenefitsTransferInResponse> CalculateAsync(CapitalBenefitTransferInComparerRequest request);

    Task<PensionVersusCapitalResponse> CalculateAsync(PensionVersusCapitalRequest request);
}
