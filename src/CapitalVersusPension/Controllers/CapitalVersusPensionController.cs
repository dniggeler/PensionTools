using System.Threading.Tasks;
using CapitalVersusPension.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CapitalVersusPension.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CapitalVersusPensionController : ControllerBase
    {
        private readonly IIncomeTaxCalculator _incomeTaxCalculator;
        private readonly ILogger<CapitalVersusPensionController> _logger;

        public CapitalVersusPensionController(IIncomeTaxCalculator incomeTaxCalculator,
            ILogger<CapitalVersusPensionController> logger)
        {
            _incomeTaxCalculator = incomeTaxCalculator;
            _logger = logger;
        }

        [HttpPost]
        public Task<TaxResult> Calculate([FromBody] TaxPerson taxPerson)
        {
            return _incomeTaxCalculator.CalculateAsync(taxPerson);
        }
    }
}
