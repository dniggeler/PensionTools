using Application.Features.TaxScenarios.Models;
using Domain.Enums;
using Domain.Models.Tax;
using Swashbuckle.AspNetCore.Filters;

namespace TaxCalculator.WebApi.Examples;

public class PensionVersusCapitalRequestExample : IExamplesProvider<PensionVersusCapitalRequest>
{
    public PensionVersusCapitalRequest GetExamples()
    {
        return new PensionVersusCapitalRequest
        {
            CalculationYear = 2019,
            MunicipalityId = 261,
            Canton = Canton.ZH,
            RetirementPension = 50_000,
            RetirementCapital = 500_000,
            TaxPerson = new TaxPerson
            {
                Name = "Test",
                CivilStatus = CivilStatus.Married,
                ReligiousGroupType = ReligiousGroupType.Other,
                PartnerReligiousGroupType = ReligiousGroupType.Other,
                TaxableIncome = 28_800,
                TaxableFederalIncome = 28_800,
                TaxableWealth = 500_000,
            }
        };
    }
}
