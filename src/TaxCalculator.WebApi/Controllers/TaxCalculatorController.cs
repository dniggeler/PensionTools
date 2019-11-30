using System;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using TaxCalculator.WebApi.Models;
using static LanguageExt.Prelude;

namespace TaxCalculator.WebApi.Controllers
{
    [ApiController]
    [Route("api/calculators/tax")]
    public class TaxCalculatorController : ControllerBase
    {
        private readonly ICapitalBenefitTaxCalculator benefitTaxCalculator;
        private readonly IFullCapitalBenefitTaxCalculator fullCapitalBenefitTaxCalculator;
        private readonly IFullTaxCalculator fullTaxCalculator;
        private readonly ILogger<TaxCalculatorController> logger;

        public TaxCalculatorController(
            ICapitalBenefitTaxCalculator benefitTaxCalculator,
            IFullCapitalBenefitTaxCalculator fullCapitalBenefitTaxCalculator,
            IFullTaxCalculator fullTaxCalculator,
            ILogger<TaxCalculatorController> logger)
        {
            this.benefitTaxCalculator = benefitTaxCalculator;
            this.fullCapitalBenefitTaxCalculator = fullCapitalBenefitTaxCalculator;
            this.fullTaxCalculator = fullTaxCalculator;
            this.logger = logger;
        }

        [HttpPost]
        [Route("full/incomeandwealth")]
        public async Task<ActionResult<FullTaxResponse>> CalculateFullTax(FullTaxRequest request)
        {
            if (request == null)
            {
                return this.BadRequest(nameof(request));
            }

            var taxPerson = MapRequest();

            Either<string, FullTaxResult> result =
                await this.fullTaxCalculator.CalculateAsync(
                    request.CalculationYear,
                    taxPerson);

            return result
                .Match<ActionResult>(
                    Right: r => this.Ok(MapResponse(r)),
                    Left: this.BadRequest);

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
                    CantonRate = r.StateTaxResult.CantonRate,
                    MunicipalityRate = r.StateTaxResult.MunicipalityRate,
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
                    Canton = request.Canton,
                    CivilStatus = request.CivilStatus,
                    Municipality = request.Municipality,
                    ReligiousGroupType = request.ReligiousGroup,
                    PartnerReligiousGroupType = request.PartnerReligiousGroup ?? ReligiousGroupType.Other,
                    NumberOfChildren = 0,
                    TaxableIncome = request.TaxableIncome,
                    TaxableWealth = request.TaxableWealth,
                    TaxableFederalIncome = request.TaxableFederalIncome,
                };
            }
        }

        [HttpPost]
        [Route("state/capitalbenefit")]
        public async Task<ActionResult<CapitalBenefitTaxResponse>> CalculateCapitalBenefitTax(
            CapitalBenefitTaxRequest request)
        {
            if (request == null)
            {
                return this.BadRequest(nameof(request));
            }

            var taxPerson = MapRequest();

            Either<string, CapitalBenefitTaxResult> result =
                await this.benefitTaxCalculator.CalculateAsync(
                    request.CalculationYear,
                    taxPerson);

            return result
                .Match<ActionResult>(
                Right: r => this.Ok(MapResponse(r)),
                Left: this.BadRequest);

            // local methods
            CapitalBenefitTaxResponse MapResponse(CapitalBenefitTaxResult r)
            {
                return new CapitalBenefitTaxResponse
                {
                    Name = taxPerson.Name,
                    CalculationYear = request.CalculationYear,
                    TaxAmount = r.TotalTaxAmount,
                    ChurchTaxAmount = r.ChurchTaxAmount,
                    CantonRate = r.CantonRate,
                    MunicipalityRate = r.MunicipalityRate,
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
                    Canton = request.Canton,
                    CivilStatus = request.CivilStatus,
                    Municipality = request.Municipality,
                    ReligiousGroupType = request.ReligiousGroup,
                    PartnerReligiousGroupType = request.PartnerReligiousGroup ?? ReligiousGroupType.Other,
                    NumberOfChildren = 0,
                    TaxableBenefits = request.TaxableBenefits,
                };
            }
        }

        [HttpPost]
        [Route("full/capitalbenefit")]
        public async Task<ActionResult<CapitalBenefitTaxResponse>> CalculateFullCapitalBenefitTax(
            CapitalBenefitTaxRequest request)
        {
            if (request == null)
            {
                return this.BadRequest(nameof(request));
            }

            var taxPerson = MapRequest();

            Either<string, FullCapitalBenefitTaxResult> result =
                await this.fullCapitalBenefitTaxCalculator.CalculateAsync(
                    request.CalculationYear,
                    taxPerson);

            return result
                .Match<ActionResult>(
                Right: r => this.Ok(MapResponse(r)),
                Left: this.BadRequest);

            // local methods
            CapitalBenefitTaxResponse MapResponse(FullCapitalBenefitTaxResult r)
            {
                return new CapitalBenefitTaxResponse
                {
                    Name = taxPerson.Name,
                    CalculationYear = request.CalculationYear,
                    TaxAmount = r.TotalTaxAmount,
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
                    Canton = request.Canton,
                    CivilStatus = request.CivilStatus,
                    Municipality = request.Municipality,
                    ReligiousGroupType = request.ReligiousGroup,
                    PartnerReligiousGroupType = request.PartnerReligiousGroup ?? ReligiousGroupType.Other,
                    NumberOfChildren = 0,
                    TaxableBenefits = request.TaxableBenefits,
                };
            }
        }
    }
}
