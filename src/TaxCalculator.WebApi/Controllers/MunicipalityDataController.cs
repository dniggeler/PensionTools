using System.Collections.Generic;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

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
        /// <remarks>
        /// tbd. detailierte Beschreibung auf Deutsch.
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<string>> Get()
        {
            Either<string, IEnumerable<MunicipalityModel>> result = new List<MunicipalityModel>
            {
                new MunicipalityModel
                {
                    BfsNumber = 261,
                    Canton = Canton.ZH,
                    Name = "Zürich",
                },
                new MunicipalityModel
                {
                    BfsNumber = 3,
                    Canton = Canton.ZH,
                    Name = "Langnau a.A.",
                },
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
        /// <remarks>
        /// tbd. detailierte Beschreibung auf Deutsch.
        /// </remarks>
        [HttpGet("{id}", Name = "Get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<MunicipalityModel> Get(int id)
        {
            Either<string, MunicipalityModel> result =
                Prelude.Right<MunicipalityModel>(new MunicipalityModel
                {
                    BfsNumber = 261,
                    Canton = Canton.ZH,
                    Name = "Zürich",
                });

            return result
                .Match<ActionResult>(
                    Right: r => this.Ok(r),
                    Left: this.BadRequest);
        }
    }
}