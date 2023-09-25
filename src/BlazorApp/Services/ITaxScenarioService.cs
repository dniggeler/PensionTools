using System.Threading.Tasks;
using Application.Features.TaxScenarios.Models;
using PensionCoach.Tools.CommonTypes.Features.PensionVersusCapital;
using PensionCoach.Tools.CommonTypes.MultiPeriod;

namespace BlazorApp.Services;

public interface ITaxScenarioService
{
    Task<ScenarioCalculationResponse> CalculateAsync(CapitalBenefitTransferInComparerRequest request);

    Task<ScenarioCalculationResponse> CalculateAsync(PensionVersusCapitalRequest request);
}
