using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Municipality;

namespace TaxCalculator.WebApi.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/data/municipality")]
    public class MunicipalityDataController : ControllerBase
    {
        private readonly IMunicipalityConnector municipalityConnector;
        private readonly ILogger<MunicipalityDataController> logger;

        public MunicipalityDataController(
            IMunicipalityConnector municipalityConnector,
            ILogger<MunicipalityDataController> logger)
        {
            this.municipalityConnector = municipalityConnector;
            this.logger = logger;
        }

        /// <summary>
        /// Gets municipality data supported by the calculators.
        /// </summary>
        /// <returns>A list of municipalities.</returns>
        /// <remarks>
        /// Holt die vollständige Gemeindeliste.
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            return this.Ok(await this.municipalityConnector.GetAllAsync());
        }

        /// <summary>
        /// Gets a municipality by its BFS id.
        /// </summary>
        /// <param name="year">Year of validity.</param>
        /// <param name="id">BFS municipality number, e.g. 261 for city of Zurich.</param>
        /// <returns>Detailed municipality data.</returns>
        /// <remarks>
        /// Holt Steuerdetails der Gemeinde. Als Schlüssel dient
        /// die BFS-Gemeindenummer. Diese Nummer kann für eine Gemeinde
        /// über die Zeit ändern.
        /// </remarks>
        [HttpGet("{year}/{id}", Name = "Get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MunicipalityModel>> Get(int year, int id)
        {
            Either<string, MunicipalityModel> result =
                await this.municipalityConnector.GetAsync(id, year);

            return result
                .Match<ActionResult>(
                    Right: this.Ok,
                    Left: this.NotFound);
        }

        /// <summary>
        /// Search the municipality directory including its history
        /// matching filter values.
        /// </summary>
        /// <returns>A list of municipalities.</returns>
        /// <remarks>
        /// Durchsucht das Gemeindeverzeichnis gemäss Filterwerte.
        /// </remarks>
        [HttpPost]
        [Route("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<MunicipalityModel>> Search(
            MunicipalitySearchFilter filter)
        {
            if (filter == null)
            {
                return this.BadRequest("Search filter is null");
            }

            IEnumerable<MunicipalityModel> result =
                this.municipalityConnector.Search(filter);

            return this.Ok(result);
        }
    }
}