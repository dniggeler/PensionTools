using System.Threading.Tasks;
using Application.Bvg;
using Application.Bvg.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaxCalculator.WebApi.Models.Bvg;

namespace TaxCalculator.WebApi.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/calculator/bvg")]
    public class BvgCalculatorController : ControllerBase
    {
        private readonly IBvgCalculator bvgCalculator;
        private readonly ILogger<TaxCalculatorController> logger;

        public BvgCalculatorController(
            IBvgCalculator bvgCalculator,
            ILogger<TaxCalculatorController> logger)
        {
            this.bvgCalculator = bvgCalculator;
            this.logger = logger;
        }

        /// <summary>
        /// Calculates the BVG benefits at retirement date of the person.
        /// </summary>
        /// <param name="request">The request includes details about the person.</param>
        /// <returns>Calculation results.</returns>
        /// <response code="200">If calculation is successful.</response>
        /// <response code="400">
        /// If request is incomplete or cannot be validated.
        /// </response>
        /// <remarks>
        /// Berechnet die BVG Leistungen zum ordentlichen Pensionierungsalter.
        /// </remarks>
        [HttpPost]
        [Route("benefits")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BvgCalculationResult>> Calculate(BvgCalculationRequest request)
        {
            if (request == null)
            {
                return BadRequest(nameof(request));
            }

            var bvgPerson = MapBvgPerson();
            var capital = new PredecessorRetirementCapital
            {
                BeginOfYearAmount = request.RetirementCapitalBeginOfYear,
                DateOfProcess = request.DateOfCalculation,
                CurrentAmount = request.RetirementCapitalBeginOfYear,
                EndOfYearAmount = request.RetirementCapitalEndOfYear,
            };

            var result =
                await bvgCalculator.CalculateAsync(capital, request.DateOfCalculation, bvgPerson);

            return result
                .Match<ActionResult>(
                    Right: Ok,
                    Left: BadRequest);

            // local methods
            BvgPerson MapBvgPerson()
            {
                return new BvgPerson
                {
                    DateOfBirth = request.DateOfBirth,
                    ReportedSalary = request.Salary,
                    Gender = request.Gender,
                    PartTimeDegree = decimal.One,
                };
            }
        }
    }
}
