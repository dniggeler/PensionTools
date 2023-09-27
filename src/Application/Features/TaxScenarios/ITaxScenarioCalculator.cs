using Domain.Models.Scenarios;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Features.TaxScenarios;

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

    Task<Either<string, ScenarioCalculationResult>> PensionVersusCapitalComparisonAsync(
        int calculationYear,
        int municipalityId,
        decimal yearConsumptionAmount,
        decimal retirementPension,
        decimal retirementCapital,
        decimal netWealthReturn,
        TaxPerson taxPerson);
}
