using System.Threading.Tasks;
using Calculators.CashFlow.Models;
using Domain.Models.Tax;
using LanguageExt;

namespace Calculators.CashFlow;

public interface ITaxScenarioCalculator
{
    /// <param name="startingYear"></param>
    /// <param name="bfsMunicipalityId"></param>
    /// <param name="person"></param>
    /// <param name="scenarioModel"></param>
    /// <returns></returns>
    Task<Either<string, ScenarioCalculationResult>> CapitalBenefitTransferInsAsync(
        int startingYear,
        int bfsMunicipalityId,
        TaxPerson person,
        CapitalBenefitTransferInsScenarioModel scenarioModel);

    Task<Either<string, ScenarioCalculationResult>> ThirdPillarVersusSelfInvestmentAsync(
        int startingYear,
        int bfsMunicipalityId,
        TaxPerson person,
        ThirdPillarVersusSelfInvestmentScenarioModel scenarioModel);
}
