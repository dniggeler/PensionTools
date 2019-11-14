using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using TaxCalculator.WebApi.Models;


namespace TaxCalculator.WebApi.Controllers
{
    [ApiController]
    [Route("api/tax/calculators")]
    public class TaxCalculatorController : ControllerBase
    {
        private readonly ICapitalBenefitTaxCalculator _benefitTaxCalculator;
        private readonly ILogger<TaxCalculatorController> _logger;

        public TaxCalculatorController(
            ICapitalBenefitTaxCalculator benefitTaxCalculator,
            ILogger<TaxCalculatorController> logger)
        {
            _benefitTaxCalculator = benefitTaxCalculator;
            _logger = logger;
        }

        [HttpPost]
        [Route("capitalbenefit")]
        public async Task<ActionResult<CapitalBenefitTaxResponse>> CalculateCapitalBenefitTax(
            CapitalBenefitTaxRequest request)
        {
            var result = await _benefitTaxCalculator.CalculateAsync(
                request.CalculationYear,
                new CapitalBenefitTaxPerson
                {
                    Name ="Burli",
                    Canton = "ZH",
                    CivilStatus = CivilStatus.Single,
                    Municipality = "Zürich",
                    ReligiousGroupType = ReligiousGroupType.Protestant,
                    NumberOfChildren = 0,
                    TaxableBenefits = 2000000,
                });
            var response = await Task.FromResult(new CapitalBenefitTaxResponse
            {
                TaxAmount =  result.Match(
                    Right: r => r.TotalTaxAmount,
                    Left: v => 0M)
            });

            return Ok(response);
        }
    }
}
