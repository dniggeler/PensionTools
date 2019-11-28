using System;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using TaxCalculator.WebApi.Models;


namespace TaxCalculator.WebApi.Controllers
{
    [ApiController]
    [Route("api/calculators/tax")]
    public class TaxCalculatorController : ControllerBase
    {
        private readonly ICapitalBenefitTaxCalculator _benefitTaxCalculator;
        private readonly IFullCapitalBenefitTaxCalculator _fullCapitalBenefitTaxCalculator;
        private readonly IFullTaxCalculator _fullTaxCalculator;
        private readonly ILogger<TaxCalculatorController> _logger;

        public TaxCalculatorController(
            ICapitalBenefitTaxCalculator benefitTaxCalculator,
            IFullCapitalBenefitTaxCalculator fullCapitalBenefitTaxCalculator,
            IFullTaxCalculator fullTaxCalculator,
            ILogger<TaxCalculatorController> logger)
        {
            _benefitTaxCalculator = benefitTaxCalculator;
            _fullCapitalBenefitTaxCalculator = fullCapitalBenefitTaxCalculator;
            _fullTaxCalculator = fullTaxCalculator;
            _logger = logger;
        }

        [HttpPost]
        [Route("full/incomeandwealth")]
        public async Task<ActionResult<FullTaxResponse>> CalculateFullTax(
            FullTaxRequest request)
        {
            var taxPerson = MapRequest();

            Either<string, FullTaxResult> result =
                await _fullTaxCalculator.CalculateAsync(
                    request.CalculationYear,
                    taxPerson);

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
            var taxPerson = MapRequest();

            Either<string, CapitalBenefitTaxResult> result =
                await _benefitTaxCalculator.CalculateAsync(
                    request.CalculationYear,
                    taxPerson);

            return result
                .Match<ActionResult>(
                Right: r => Ok(MapResponse(r)),
                Left: BadRequest);

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
            var taxPerson = MapRequest();

            Either<string, FullCapitalBenefitTaxResult> result =
                await _fullCapitalBenefitTaxCalculator.CalculateAsync(
                    request.CalculationYear,
                    taxPerson);

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
