using System;
using System.Threading.Tasks;
using Calculators.CashFlow;
using Calculators.CashFlow.Models;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.TaxCalculator.Abstractions;

namespace TaxCalculator.WebApi.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/calculator")]
    public class MultiPeriodCalculatorController : ControllerBase
    {
        private readonly IMultiPeriodCashFlowCalculator multiPeriodCashFlowCalculator;
        private readonly IMunicipalityConnector municipalityResolver;

        public MultiPeriodCalculatorController(
            IMultiPeriodCashFlowCalculator multiPeriodCashFlowCalculator,
            IMunicipalityConnector municipalityResolver)
        {
            this.multiPeriodCashFlowCalculator = multiPeriodCashFlowCalculator;
            this.municipalityResolver = municipalityResolver;
        }

        /// <summary>
        /// Calculates for a pre-defined set of accounts, cash-flows and clear-actions a tax-person's wealth development
        /// with respect to her tax situation.
        /// </summary>
        /// <param name="request">The request includes definitions for accounts, cash-flows and clear-actions for a the tax person.</param>
        /// <returns>Calculation results.</returns>
        /// <response code="200">If calculation is successful.</response>
        /// <response code="400">
        /// If request is incomplete or cannot be validated. The calculator may also not support all cantons.
        /// </response>
        /// <remarks>
        /// Definitionen von Konten, Cash-Flows und Clear-Actions für eine steuerbare Person bestimmen wie sich deren Vermögen über die Zeit
        /// entwickelt. Die Berechnung berücksichtigt insbesondere Steuereffekte auf den einzelnen Konti.
        /// </remarks>
        [HttpPost]
        [Route("multiperiod")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MultiPeriodResponse>> CalculateMultiPeriodCashFlows(MultiPeriodRequest request)
        {
            if (request == null)
            {
                return BadRequest(nameof(request));
            }

            MultiPeriodCalculatorPerson person = MapPerson();

            Either<string, MunicipalityModel> municipalityData =
                await municipalityResolver.GetAsync(request.BfsMunicipalityId, request.StartingYear);

            Either<string, MultiPeriodCalculationResult> result = await municipalityData
                .BindAsync(m => multiPeriodCashFlowCalculator.CalculateAsync(
                    request.StartingYear, request.NumberOfPeriods, person with { Canton = m.Canton }, request.CashFlowDefinitionHolder));

            return result
                .Match<ActionResult>(
                    Right: r => Ok(MapResponse(r)),
                    Left: BadRequest);

            // local methods
            MultiPeriodCalculatorPerson MapPerson()
            {
                var name = string.IsNullOrEmpty(request.Name)
                    ? Guid.NewGuid().ToString().Substring(0, 6)
                    : request.Name;

                return new MultiPeriodCalculatorPerson
                {
                    Name = name,
                    MunicipalityId = request.BfsMunicipalityId,
                    CivilStatus = request.CivilStatus,
                    ReligiousGroupType = request.ReligiousGroupType,
                    PartnerReligiousGroupType = request.PartnerReligiousGroupType ?? ReligiousGroupType.Other,
                    NumberOfChildren = 0,
                    Income = request.Income,
                    Wealth = request.Wealth,
                    CapitalBenefits = (request.CapitalBenefitsPillar3A, request.CapitalBenefitsPension),
                };
            }

            MultiPeriodResponse MapResponse(MultiPeriodCalculationResult calculationResult)
            {
                return new MultiPeriodResponse
                {
                    StartingYear = calculationResult.StartingYear,
                    NumberOfPeriods = calculationResult.NumberOfPeriods,
                    Accounts = calculationResult.Accounts,
                };
            }
        }
    }
}
