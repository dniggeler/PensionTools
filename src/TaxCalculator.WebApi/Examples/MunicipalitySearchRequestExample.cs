using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Municipality;
using Swashbuckle.AspNetCore.Filters;

namespace TaxCalculator.WebApi.Examples
{
    public class MunicipalitySearchRequestExample : IExamplesProvider<MunicipalitySearchFilter>
    {
        public MunicipalitySearchFilter GetExamples()
        {
            return new MunicipalitySearchFilter
            {
                Name = "R",
                Canton = Canton.ZH,
                YearOfValidity = 2019,
            };
        }
    }
}
