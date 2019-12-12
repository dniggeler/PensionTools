using System.Collections.Generic;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.TaxCalculator.Abstractions;

namespace TaxCalculator.WebApi.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/data/municipality")]
    public class MunicipalityDataController : ControllerBase
    {
        private readonly ILogger<MunicipalityDataController> logger;

        public MunicipalityDataController(
            IFullCapitalBenefitTaxCalculator fullCapitalBenefitTaxCalculator,
            IFullTaxCalculator fullTaxCalculator,
            ILogger<MunicipalityDataController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Gets municipality data supported by the calculators.
        /// </summary>
        /// <returns>A list of municipalities.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<string>> Get()
        {
            Either<string, IEnumerable<string>> result = new List<string>
            {
                "Langnau a.A.",
                "Zürich",
            };

            return result
                .Match<ActionResult>(
                    Right: r => this.Ok(r),
                    Left: this.BadRequest);
        }

        /// <summary>
        /// Gets a municipality by its BFS id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Detailed municipality data.</returns>
        [HttpGet("{id}", Name = "Get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string> Get(int id)
        {
            Either<string, string> result = Prelude.Right<string>("Langnau a.A.");

            return result
                .Match<ActionResult>(
                    Right: r => this.Ok(r),
                    Left: this.BadRequest);
        }
    }
}