using System;
using System.Threading.Tasks;
using Calculators.CashFlow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxComparison;

namespace TaxCalculator.WebApi.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/scenario/tax")]
public class TaxScenarioController : ControllerBase
{
    private readonly ITaxScenarioCalculator scenarioCalculator;

    public TaxScenarioController(ITaxScenarioCalculator scenarioCalculator)
    {
        this.scenarioCalculator = scenarioCalculator;
    }

    /// <summary>
    /// Calculates the wealth over multiple period for one or multiple transfer-ins into a pension plan or third pillar account.
    /// </summary>
    /// <param name="request">The request includes details about the tax person and the desired scenario.</param>
    /// <returns>Multi-period calculation results.</returns>
    /// <response code="200">If calculation is successful.</response>
    /// <response code="400">
    /// If request is incomplete or cannot be validated.
    /// </response>
    /// <remarks>
    /// Berechnet die Wertentwicklung von ein oder mehreren Kapitaleinlagen in die Pensionkasse oder auf ein 3a Konto.
    /// Dabei werden steuerliche Aspekte bei der Einlage (Abzugsmöglichkeit) sowie die anfallende Kapitalbezugssteuer beim Bezug der
    /// Kapitalleistungen berücksichtigt.
    /// </remarks>
    [HttpPost]
    [Route(nameof(CalculateTransferInCapitalBenefits))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MultiPeriodResponse>> CalculateTransferInCapitalBenefits(CapitalBenefitTransferInComparerRequest request)
    {
        if (request == null)
        {
            return BadRequest(nameof(request));
        }

        var taxPerson = MapPerson();
        var scenarioModel = MapScenarioModel();

        var response = new MultiPeriodResponse();

        await scenarioCalculator.TransferInCapitalBenefitsAsync(
            request.CalculationYear, request.BfsMunicipalityId, taxPerson, scenarioModel)
            .IterAsync(r =>
            {
                response.NumberOfPeriods = r.NumberOfPeriods;
                response.StartingYear = r.StartingYear;
                response.Accounts = r.Accounts;
            });

        return response;

        TransferInCapitalBenefitsScenarioModel MapScenarioModel()
        {
            return new TransferInCapitalBenefitsScenarioModel
            {
                TransferIns = request.TransferIns,
                NetReturnRate = request.NetReturnRate,
                WithCapitalBenefitTaxation = request.WithCapitalBenefitTaxation,
                YearOfCapitalBenefitWithdrawal = request.YearOfCapitalBenefitWithdrawal,
                FinalRetirementCapital = request.FinalRetirementCapital
            };
        }

        TaxPerson MapPerson()
        {
            var name = string.IsNullOrEmpty(request.Name)
                ? Guid.NewGuid().ToString()[..6]
                : request.Name;

            return new TaxPerson
            {
                Name = name,
                CivilStatus = request.CivilStatus,
                ReligiousGroupType = request.ReligiousGroup,
                PartnerReligiousGroupType = request.PartnerReligiousGroup ?? ReligiousGroupType.Other,
                NumberOfChildren = 0,
                TaxableFederalIncome = request.TaxableFederalIncome,
                TaxableIncome = request.TaxableIncome,
                TaxableWealth = request.TaxableWealth,
            };
        }
    }
}
