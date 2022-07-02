using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.PostOpenApi.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions;

namespace TaxCalculator.WebApi.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/data/municipality")]
    public class MunicipalityDataController : ControllerBase
    {
        private readonly IMunicipalityConnector municipalityConnector;

        public MunicipalityDataController(
            IMunicipalityConnector municipalityConnector)
        {
            this.municipalityConnector = municipalityConnector;
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
            return Ok(await municipalityConnector.GetAllAsync());
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
                await municipalityConnector.GetAsync(id, year);

            return result
                .Match<ActionResult>(
                    Right: Ok,
                    Left: NotFound);
        }

        /// <summary>
        /// Search the municipality directory including its history
        /// matching filter values.
        /// </summary>
        /// <param name="filter">Search municipalities with respect to given filter.</param>
        /// <returns>A list of municipalities.</returns>
        /// <remarks>
        /// Durchsucht das Gemeindeverzeichnis gemäss Filterwerte.
        /// </remarks>
        [HttpPost]
        [Route("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<MunicipalityModel>> Search(MunicipalitySearchFilter filter)
        {
            if (filter == null)
            {
                return BadRequest("Search filter is null");
            }

            IEnumerable<MunicipalityModel> result =
                municipalityConnector.Search(filter);

            return Ok(result);
        }

        /// <summary>
        /// All Swiss zip codes supplied by the Swiss Post Open Api.
        /// </summary>
        /// <remarks>
        /// Vollständiges PLZ-Verzeichnis bereitgestellt von der Post.
        /// </remarks>
        [HttpGet]
        [Route("zip")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ZipModel>>> GetZipCodes()
        {
            List<ZipModel> result = new List<ZipModel>();
            await foreach (var model in municipalityConnector.GetAllZipCodesAsync(int.MaxValue))
            {
                result.Add(model);
            }

            return Ok(result);
        }

        /// <summary>
        /// Populate internal data store with all Swiss zip codes supplied by the Swiss Post Open Api.
        /// </summary>
        /// <remarks>
        /// Vollständiges PLZ-Verzeichnis bereitgestellt von der Post abspeichern.
        /// </remarks>
        [HttpPost]
        [Route("zip/populate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> PopulateZipCodes()
        {
            int count = await municipalityConnector.PopulateWithZipCodeAsync();

            return Ok(count);
        }

        /// <summary>
        /// Populate internal data store with all Swiss zip codes supplied by the Swiss Post Open Api.
        /// </summary>
        /// <remarks>
        /// Vollständiges PLZ-Verzeichnis bereitgestellt von der Post abspeichern.
        /// </remarks>
        [HttpPost]
        [Route("tax/populate/{clear:bool}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> PopulateTaxLocation(bool clear)
        {
            int count = await municipalityConnector.PopulateWithTaxLocationAsync(clear);

            return Ok(count);
        }

        /// <summary>
        /// Populate internal data store with all Swiss zip codes supplied by the Swiss Post Open Api.
        /// </summary>
        /// <remarks>
        /// Vollständiges PLZ-Verzeichnis bereitgestellt von der Post abspeichern.
        /// </remarks>
        [HttpPost]
        [Route("zip/stage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> PopulateStageMunicipalityTable()
        {
            int count = await municipalityConnector.StagePlzTableAsync();

            return Ok(count);
        }

        [HttpPost]
        [Route("tax/clean")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> CleanMunicipalityNames()
        {
            int count = await municipalityConnector.CleanMunicipalityName();

            return Ok(count);
        }
    }
}
