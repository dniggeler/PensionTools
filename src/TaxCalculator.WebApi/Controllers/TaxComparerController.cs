using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxComparison;
using Tax.Tools.Comparison.Abstractions;

namespace TaxCalculator.WebApi.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/comparer/tax")]
    public class TaxComparerController : ControllerBase
    {
        private readonly ITaxComparer taxComparer;

        public TaxComparerController(ITaxComparer taxComparer)
        {
            this.taxComparer = taxComparer;
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
        public async IAsyncEnumerable<CapitalBenefitTaxComparerResponse> CompareCapitalBenefitTax(CapitalBenefitTaxComparerRequest request)
        {
            if (request == null)
            {
                yield break;
            }

            var taxPerson = MapRequest();
            await foreach (var taxResult in taxComparer.CompareCapitalBenefitTaxAsync(taxPerson, request.BfsNumberList))
            {
                if (taxResult.IsLeft)
                {
                    continue;
                }

                var compareResult = new CapitalBenefitTaxComparerResponse
                {
                    Name = taxPerson.Name,
                };

                taxResult.Iter(m =>
                {
                    compareResult.MunicipalityId = m.MunicipalityId;
                    compareResult.MunicipalityName = m.MunicipalityName;
                    compareResult.Canton = m.Canton;
                    compareResult.MaxSupportedTaxYear = m.MaxSupportedTaxYear;
                    compareResult.TotalTaxAmount = m.MunicipalityTaxResult.TotalTaxAmount;
                    compareResult.TaxDetails = new TaxAmountDetail
                    {
                        CantonTaxAmount = m.MunicipalityTaxResult.StateResult.CantonTaxAmount,
                        MunicipalityTaxAmount = m.MunicipalityTaxResult.StateResult.MunicipalityTaxAmount,
                        FederalTaxAmount = m.MunicipalityTaxResult.FederalResult.TaxAmount,
                        ChurchTaxAmount = m.MunicipalityTaxResult.StateResult.ChurchTaxAmount,
                    };
                });

                yield return compareResult;
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
                    TaxableCapitalBenefits = request.TaxableBenefits,
                };
            }
        }
    }
}
