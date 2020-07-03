using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Tax.Tools.Comparison.Abstractions;
using Tax.Tools.Comparison.Abstractions.Models;
using TaxCalculator.WebApi.Models;

namespace TaxCalculator.WebApi.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/comparer/tax")]
    public class TaxComparerController : ControllerBase
    {
        private readonly ITaxComparer taxComparer;
        private readonly ILogger<TaxCalculatorController> logger;

        public TaxComparerController(
            ITaxComparer taxComparer,
            ILogger<TaxCalculatorController> logger)
        {
            this.taxComparer = taxComparer;
            this.logger = logger;
        }

        /// <summary>
        /// Calculates the overall capital benefit tax for all available municipalities
        /// for a given calculations year.
        /// </summary>
        /// <param name="request">The request includes details about the tax person and the taxable amount.</param>
        /// <returns>Calculation results.</returns>
        /// <response code="200">If calculation is successful.</response>
        /// <response code="400">
        /// If request is incomplete or cannot be validated. The calculator may also not support all cantons.
        /// </response>
        /// <remarks>
        /// Berechnet die Bundes-, Staats- und Gemeindesteuern auf Kapitalleistungen (Kapitalbezugssteuer) für alle Gemeinden in der Datenbasis
        /// für ein vorgegebenes Steuerjahr.
        /// </remarks>
        [HttpPost]
        [Route("capitalbenefit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CapitalBenefitTaxComparerResponse>> CompareCapitalBenefitTax(
            CapitalBenefitTaxComparerRequest request)
        {
            if (request == null)
            {
                return this.BadRequest(nameof(request));
            }

            var taxPerson = MapRequest();

            var result =
                await this.taxComparer
                    .CompareCapitalBenefitTaxAsync(request.CalculationYear, taxPerson);

            return result
                .Match<ActionResult>(
                    Right: r => this.Ok(MapResponse(r)),
                    Left: this.BadRequest);

            // local methods
            IReadOnlyCollection<CapitalBenefitTaxComparerResponse> MapResponse(IReadOnlyCollection<CapitalBenefitTaxComparerResult> resultList)
            {
                return resultList.Map(r => new CapitalBenefitTaxComparerResponse
                    {
                        Name = taxPerson.Name,
                        MunicipalityId = r.MunicipalityId,
                        MunicipalityName = r.MunicipalityName,
                        CalculationYear = request.CalculationYear,
                        TotalTaxAmount = r.MunicipalityTaxResult.TotalTaxAmount,
                        TaxDetails = new TaxAmountDetail
                        {
                            CantonTaxAmount = r.MunicipalityTaxResult.StateResult.CantonTaxAmount,
                            FederalTaxAmount = r.MunicipalityTaxResult.FederalResult.TaxAmount,
                        },
                    })
                    .ToList();
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
    }
}