using Domain.Enums;
using Domain.Models.Municipality;
using PensionCoach.Tools.CommonTypes;
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
