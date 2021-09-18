using System.Threading.Tasks;
using Calculators.CashFlow;
using Calculators.CashFlow.Models;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaxCalculator.WebApi.Models;

namespace TaxCalculator.WebApi.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/calculators")]
    public class MultiPeriodCalculatorController : ControllerBase
    {
        private readonly IMultiPeriodCashFlowCalculator multiPeriodCashFlowCalculator;
        private readonly ILogger<TaxCalculatorController> logger;

        public MultiPeriodCalculatorController(
            IMultiPeriodCashFlowCalculator multiPeriodCashFlowCalculator,
            ILogger<TaxCalculatorController> logger)
        {
            this.multiPeriodCashFlowCalculator = multiPeriodCashFlowCalculator;
            this.logger = logger;
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

            Either<string, MultiPeriodCalculationResult> result = await multiPeriodCashFlowCalculator.CalculateAsync(
                request.StartingYear, request.NumberOfPeriods, request.Person, request.CashFlowDefinitionHolder);

            return result
                .Match<ActionResult>(
                    Right: r => Ok(MapResponse(r)),
                    Left: BadRequest);

            // local methods
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
