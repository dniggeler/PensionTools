using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Swashbuckle.AspNetCore.Filters;
using TaxCalculator.WebApi.Models;

namespace TaxCalculator.WebApi.Examples
{
    public class FullTaxRequestExample : IExamplesProvider<FullTaxRequest>
    {
        public FullTaxRequest GetExamples()
        {
            return new FullTaxRequest
            {
                Name = "Test",
                CalculationYear = 2018,
                Canton = Canton.ZH,
                CivilStatus = CivilStatus.Single,
                Municipality = "Zürich",
                ReligiousGroup = ReligiousGroupType.Other,
                TaxableIncome = 99995M,
                TaxableFederalIncome = 99995M,
                TaxableWealth = 522000,
            };
        }
    }
}