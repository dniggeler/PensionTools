using Domain.Enums;
using Domain.Models.Tax;
using PensionCoach.Tools.CommonTypes.Features.PensionVersusCapital;
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
            RetirementPension = 50_000,
            RetirementCapital = 500_000,
            YearlyConsumptionAmount = 78_800,
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
