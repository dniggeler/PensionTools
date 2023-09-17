using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models.Municipality;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PensionCoach.Tools.TaxCalculator.Abstractions;

namespace TaxCalculator.WebApi.Controllers;

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
}
