using Domain.Enums;
using PensionCoach.Tools.CommonTypes.Tax;
using Swashbuckle.AspNetCore.Filters;

namespace TaxCalculator.WebApi.Examples
{
    public class MarginalTaxRateCurveRequestExample : IExamplesProvider<MarginalTaxRequest>
    {
        public MarginalTaxRequest GetExamples()
        {
            return new MarginalTaxRequest
            {
                Name = "Test Marginal Tax Rate Curve",
                CalculationYear = 2019,
                CivilStatus = CivilStatus.Married,
                BfsMunicipalityId = 261,
                ReligiousGroup = ReligiousGroupType.Other,
                PartnerReligiousGroup = ReligiousGroupType.Other,
                TaxableAmount = 100_000M,
                LowerSalaryLimit = 0,
                UpperSalaryLimit = 200_000,
                NumberOfSamples = 10
            };
        }
    }
}
