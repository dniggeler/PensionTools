using System.Collections.Generic;
using PensionCoach.Tools.PostOpenApi.Models;

namespace TaxCalculator.WebApi.Controllers;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PensionCoach.Tools.TaxCalculator.Abstractions;

[Produces("application/json")]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminConnector adminConnector;

    public AdminController(IAdminConnector adminConnector)
    {
        this.adminConnector = adminConnector;
    }

    /// <summary>
    /// Populate internal data store with ESTV tax location id.
    /// </summary>
    /// <remarks>
    /// Gemeindeverzeichnis mit interner ESTV Steuer-Id anreichern.
    /// </remarks>
    [HttpPost]
    [Route("tax/populate/{clear:bool}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> PopulateTaxLocation(bool clear)
    {
        int count = await adminConnector.PopulateWithTaxLocationAsync(clear);

        return Ok(count);
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
        int count = await adminConnector.PopulateWithZipCodeAsync();

        return Ok(count);
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
        await foreach (var model in adminConnector.GetAllZipCodesAsync(int.MaxValue))
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
    [Route("zip/stage")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> PopulateStageMunicipalityTable()
    {
        int count = await adminConnector.StagePlzTableAsync();

        return Ok(count);
    }

    [HttpPost]
    [Route("tax/clean")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> CleanMunicipalityNames()
    {
        int count = await adminConnector.CleanMunicipalityName();

        return Ok(count);
    }
}
