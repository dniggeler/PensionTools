using System.Threading.Tasks;
using Calculators.CashFlow.Models;
using LanguageExt;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxComparison;

namespace Calculators.CashFlow;

public interface ITaxScenarioCalculator
{
    /// <param name="startingYear"></param>
    /// <param name="bfsMunicipalityId"></param>
    /// <param name="person"></param>
    /// <param name="scenarioModel"></param>
    /// <returns></returns>
    Task<Either<string, CapitalBenefitsTransferInResult>> TransferInCapitalBenefitsAsync(
        int startingYear,
        int bfsMunicipalityId,
        TaxPerson person,
        TransferInCapitalBenefitsScenarioModel scenarioModel);
}
