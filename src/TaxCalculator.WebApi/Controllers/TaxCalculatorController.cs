using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using TaxCalculator.WebApi.Models;

namespace TaxCalculator.WebApi.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/calculators/tax")]
    public class TaxCalculatorController : ControllerBase
    {
        private readonly IFullCapitalBenefitTaxCalculator fullCapitalBenefitTaxCalculator;
        private readonly IFullTaxCalculator fullTaxCalculator;
        private readonly IMunicipalityConnector municipalityResolver;

        public TaxCalculatorController(
            IFullCapitalBenefitTaxCalculator fullCapitalBenefitTaxCalculator,
            IFullTaxCalculator fullTaxCalculator,
            IMunicipalityConnector municipalityResolver)
        {
            this.fullCapitalBenefitTaxCalculator = fullCapitalBenefitTaxCalculator;
            this.fullTaxCalculator = fullTaxCalculator;
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

            Either<string, MunicipalityModel> municipalityData =
                await municipalityResolver.GetAsync(request.BfsMunicipalityId, request.CalculationYear);

            Either<string, FullTaxResult> result = await municipalityData
                .BindAsync(m => fullTaxCalculator.CalculateAsync(
                    request.CalculationYear,
                    request.BfsMunicipalityId,
                    m.Canton,
                    taxPerson));

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

            Either<string, MunicipalityModel> municipalityData =
                await municipalityResolver.GetAsync(request.BfsMunicipalityId, request.CalculationYear);

            var result =
                await municipalityData
                    .BindAsync(m => fullCapitalBenefitTaxCalculator.CalculateAsync(
                        request.CalculationYear,
                        request.BfsMunicipalityId,
                        m.Canton,
                        taxPerson));

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
                    TaxableBenefits = request.TaxableBenefits,
                };
            }
        }

        [HttpGet]
        [Route("municipality/{year}")]
        public async Task<ActionResult<IEnumerable<TaxSupportedMunicipalityModel>>> GetSupportedMunicipalities(int year)
        {
            IReadOnlyCollection<TaxSupportedMunicipalityModel> list = await municipalityResolver.GetAllSupportTaxCalculationAsync(year);

            return Ok(list);
        }
    }
}
