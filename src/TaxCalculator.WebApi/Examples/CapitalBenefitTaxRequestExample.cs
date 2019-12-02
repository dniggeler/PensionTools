using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Swashbuckle.AspNetCore.Filters;
using TaxCalculator.WebApi.Models;

namespace TaxCalculator.WebApi.Examples
{
    public class CapitalBenefitTaxRequestExample : IExamplesProvider<CapitalBenefitTaxRequest>
    {
        public CapitalBenefitTaxRequest GetExamples()
        {
            return new CapitalBenefitTaxRequest
            {
                Name = "Test",
                CalculationYear = 2018,
                Canton = "ZH",
                CivilStatus = CivilStatus.Single,
                Municipality = "Zürich",
                ReligiousGroup = ReligiousGroupType.Other,
                TaxableBenefits = 1000_000,
            };
        }
    }
}