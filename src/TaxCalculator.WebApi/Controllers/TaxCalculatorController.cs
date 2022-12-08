using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using TaxCalculator.WebApi.Models;

namespace TaxCalculator.WebApi.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/calculators/tax")]
public class TaxCalculatorController : ControllerBase
{
    private readonly ITaxCalculatorConnector taxCalculatorConnector;
    private readonly IMarginalTaxCurveCalculatorConnector marginalTaxCurveCalculatorConnector;
    private readonly IMunicipalityConnector municipalityResolver;

    public TaxCalculatorController(
        ITaxCalculatorConnector taxCalculatorConnector,
        IMarginalTaxCurveCalculatorConnector marginalTaxCurveCalculatorConnector,
        IMunicipalityConnector municipalityResolver)
    {
        this.taxCalculatorConnector = taxCalculatorConnector;
        this.marginalTaxCurveCalculatorConnector = marginalTaxCurveCalculatorConnector;
        this.municipalityResolver = municipalityResolver;
    }

    /// <summary>
    /// Calculates the full tax. Result comprises municipality, state and federal income tax amounts.
    /// And, wealth capital tax is  calculated for municipality and state only. Wealth is not taxed on federal level.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>Calculation results.</returns>
    /// <remarks>
    /// Einkommens- und Vermögenssteuerrechner für Gemeinde- und Staatsteuern. Vermögenssteuern werden
    /// auf Bundesebene keine erhoben.
    /// </remarks>
    [HttpPost]
    [Route("full/incomeandwealth")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FullTaxResponse>> CalculateFullTax(
        FullTaxRequest request)
    {
        if (request == null)
        {
            return BadRequest(nameof(request));
        }

        var taxPerson = MapRequest();

        Either<string, FullTaxResult> result = await taxCalculatorConnector.CalculateAsync(
                request.CalculationYear, request.BfsMunicipalityId, taxPerson);

        return result
            .Match<ActionResult>(
                Right: r => Ok(MapResponse(r)),
                Left: BadRequest);

        FullTaxResponse MapResponse(FullTaxResult r)
        {
            return new FullTaxResponse
            {
                Name = taxPerson.Name,
                CalculationYear = request.CalculationYear,
                TotalTaxAmount = r.TotalTaxAmount,
                FederalTaxAmount = r.FederalTaxResult.TaxAmount,
                WealthTaxAmount = r.StateTaxResult.BasisWealthTax.TaxAmount,
                CantonTaxAmount = r.StateTaxResult.CantonTaxAmount,
                MunicipalityTaxAmount = r.StateTaxResult.MunicipalityTaxAmount,
                ChurchTaxAmount = r.StateTaxResult.ChurchTaxAmount,
                PollTaxAmount = r.StateTaxResult.PollTaxAmount.IfNone(0),
                TaxRateDetails = new TaxRateDetails
                {
                    CantonRate = r.StateTaxResult.CantonRate,
                    MunicipalityRate = r.StateTaxResult.MunicipalityRate,
                    ChurchTaxRate = r.StateTaxResult.ChurchTax.TaxRate,
                },
            };
        }

        TaxPerson MapRequest()
        {
            var name = string.IsNullOrEmpty(request.Name)
                ? Guid.NewGuid().ToString().Substring(0, 6)
                : request.Name;

            return new TaxPerson
            {
                Name = name,
                CivilStatus = request.CivilStatus,
                ReligiousGroupType = request.ReligiousGroup,
                PartnerReligiousGroupType = request.PartnerReligiousGroup ?? ReligiousGroupType.Other,
                NumberOfChildren = 0,
                TaxableIncome = request.TaxableIncome,
                TaxableWealth = request.TaxableWealth,
                TaxableFederalIncome = request.TaxableFederalIncome,
            };
        }
    }

    /// <summary>
    /// Calculates marginal income tax rate curve.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>Calculation results.</returns>
    /// <remarks>
    /// Berechnet die Grenzsteuersatzkurve für die Einkommenssteuer. Ein Punkt auf der Kurve sagt aus, wie hoch der Steueranteil
    /// für einen zusätzlichen Einkommensfranken ist.
    /// </remarks>
    [HttpPost]
    [Route("marginaltaxcurve/income")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MarginalTaxResponse>> CalculateMarginalIncomeTaxCurve(MarginalTaxRequest request)
    {
        if (request == null)
        {
            return BadRequest(nameof(request));
        }

        var taxPerson = MapRequest();

        Either<string, MarginalTaxCurveResult> result =
            await marginalTaxCurveCalculatorConnector.CalculateIncomeTaxCurveAsync(
                request.CalculationYear, request.BfsMunicipalityId, taxPerson, request.LowerSalaryLimit, request.UpperSalaryLimit, request.NumberOfSamples);

        return result
            .Match<ActionResult>(
                Right: r => Ok(MapResponse(r)),
                Left: BadRequest);

        MarginalTaxCurveResult MapResponse(MarginalTaxCurveResult r)
        {
            return r;
        }

        TaxPerson MapRequest()
        {
            var name = string.IsNullOrEmpty(request.Name)
                ? Guid.NewGuid().ToString().Substring(0, 6)
                : request.Name;

            return new TaxPerson
            {
                Name = name,
                CivilStatus = request.CivilStatus,
                ReligiousGroupType = request.ReligiousGroup,
                PartnerReligiousGroupType = request.PartnerReligiousGroup ?? ReligiousGroupType.Other,
                NumberOfChildren = 0,
                TaxableIncome = request.TaxableAmount,
                TaxableFederalIncome = request.TaxableAmount
            };
        }
    }

    /// <summary>
    /// Calculates the overall capital benefit tax including state, municipality and
    /// federal tax amounts.
    /// </summary>
    /// <param name="request">The request includes details about the tax person and the taxable amount.</param>
    /// <returns>Calculation results.</returns>
    /// <response code="200">If calculation is successful.</response>
    /// <response code="400">
    /// If request is incomplete or cannot be validated. The calculator may also not support all cantons.
    /// </response>
    /// <remarks>
    /// Steuerrechner für Bundes-, Staats- und Gemeindesteuern auf Kapitalleistungen (Kapitalbezugssteuer).
    /// </remarks>
    [HttpPost]
    [Route("full/capitalbenefit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CapitalBenefitTaxResponse>> CalculateFullCapitalBenefitTax(
        CapitalBenefitTaxRequest request)
    {
        if (request == null)
        {
            return BadRequest(nameof(request));
        }

        var taxPerson = MapRequest();

        var result = await taxCalculatorConnector.CalculateAsync(
                    request.CalculationYear, request.BfsMunicipalityId, taxPerson);

        return result
            .Match<ActionResult>(
                Right: r => Ok(MapResponse(r)),
                Left: BadRequest);

        // local methods
        CapitalBenefitTaxResponse MapResponse(FullCapitalBenefitTaxResult r)
        {
            return new CapitalBenefitTaxResponse
            {
                Name = taxPerson.Name,
                CalculationYear = request.CalculationYear,
                TotalTaxAmount = r.TotalTaxAmount,
                TaxDetails = new TaxAmountDetail
                {
                    ChurchTaxAmount = r.StateResult.ChurchTaxAmount,
                    FederalTaxAmount = r.FederalResult.TaxAmount,
                    MunicipalityTaxAmount = r.StateResult.MunicipalityTaxAmount,
                    CantonTaxAmount = r.StateResult.CantonTaxAmount,
                },
            };
        }

        CapitalBenefitTaxPerson MapRequest()
        {
            var name = string.IsNullOrEmpty(request.Name)
                ? Guid.NewGuid().ToString().Substring(0, 6)
                : request.Name;

            return new CapitalBenefitTaxPerson
            {
                Name = name,
                CivilStatus = request.CivilStatus,
                ReligiousGroupType = request.ReligiousGroup,
                PartnerReligiousGroupType = request.PartnerReligiousGroup ?? ReligiousGroupType.Other,
                NumberOfChildren = 0,
                TaxableCapitalBenefits = request.TaxableBenefits,
            };
        }
    }

    /// <summary>
    /// Calculates marginal income tax rate curve.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>Calculation results.</returns>
    /// <remarks>
    /// Berechnet die Grenzsteuersatzkurve für die Kapitalbezugssteuer (z.B. Bezüge aus 3a- oder PK-Konten).
    /// Ein Punkt auf der Kurve sagt aus, wie hoch der Steueranteil für einen Bezug eines zusätzlichen Frankens ist.
    /// </remarks>
    [HttpPost]
    [Route("marginaltaxcurve/capitalbenefits")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FullTaxResponse>> CalculateMarginalCapitalBenefitsTaxCurve(MarginalTaxRequest request)
    {
        if (request == null)
        {
            return BadRequest(nameof(request));
        }

        var taxPerson = MapRequest();

        Either<string, MarginalTaxCurveResult> result =
            await marginalTaxCurveCalculatorConnector.CalculateCapitalBenefitTaxCurveAsync(
                request.CalculationYear,
                request.BfsMunicipalityId,
                taxPerson,
                request.LowerSalaryLimit,
                request.UpperSalaryLimit,
                request.NumberOfSamples);

        return result
            .Match<ActionResult>(
                Right: r => Ok(MapResponse(r)),
                Left: BadRequest);

        MarginalTaxCurveResult MapResponse(MarginalTaxCurveResult r)
        {
            return r;
        }

        CapitalBenefitTaxPerson MapRequest()
        {
            var name = string.IsNullOrEmpty(request.Name)
                ? Guid.NewGuid().ToString()[..6]
                : request.Name;

            return new CapitalBenefitTaxPerson
            {
                Name = name,
                CivilStatus = request.CivilStatus,
                ReligiousGroupType = request.ReligiousGroup,
                PartnerReligiousGroupType = request.PartnerReligiousGroup ?? ReligiousGroupType.Other,
                NumberOfChildren = 0,
                TaxableCapitalBenefits = request.TaxableAmount,
            };
        }
    }

    [HttpGet]
    [Route("municipality")]
    public async Task<ActionResult<IEnumerable<TaxSupportedMunicipalityModel>>> GetSupportedMunicipalities()
    {
        IReadOnlyCollection<TaxSupportedMunicipalityModel> list = await municipalityResolver.GetAllSupportTaxCalculationAsync();

        return Ok(list);
    }

    /// <summary>
    /// Returns the supported tax years. List depends on the tax calculator implementation.
    /// </summary>
    /// <returns>Tax calculation years.</returns>
    /// <response code="200">If calculation is successful.</response>
    /// <remarks>
    /// Unterstütze Steuerjahre - hängt von der konkreten Steuerrechner-Implementation ab.
    /// </remarks>
    [HttpGet]
    [Route("years")]
    public async Task<ActionResult<int[]>> GetSupportedTaxYears()
    {
        var years = await municipalityResolver.GetSupportedTaxYearsAsync();

        return Ok(years);
    }
}
