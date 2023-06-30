using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PensionCoach.Tools.TaxCalculator.Abstractions;

namespace TaxCalculator.WebApi.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/check")]
public class CheckSettingsController : Controller
{
    private readonly ICheckSettingsConnector checkSettingsConnector;

    public CheckSettingsController(ICheckSettingsConnector checkSettingsConnector)
    {
        this.checkSettingsConnector = checkSettingsConnector;
    }

    [HttpGet("settings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var settings = await checkSettingsConnector.GetAsync();

        return Ok(settings);
    }
}
