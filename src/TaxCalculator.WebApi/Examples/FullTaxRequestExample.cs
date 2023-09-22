using Domain.Enums;
using PensionCoach.Tools.CommonTypes.Tax;
using Swashbuckle.AspNetCore.Filters;

namespace TaxCalculator.WebApi.Examples
{
    public class FullTaxRequestExample : IExamplesProvider<FullTaxRequest>
    {
        public FullTaxRequest GetExamples()
        {
            return new FullTaxRequest
            {
                Name = "Test",
                CalculationYear = 2019,
                CivilStatus = CivilStatus.Married,
                BfsMunicipalityId = 261,
                ReligiousGroup = ReligiousGroupType.Other,
                PartnerReligiousGroup = ReligiousGroupType.Other,
                TaxableIncome = 100_000M,
                TaxableFederalIncome = 100_000M,
                TaxableWealth = 500_000,
            };
        }
    }
}
