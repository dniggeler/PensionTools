using System.Collections.Generic;
using System.Threading.Tasks;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;

namespace BlazorApp.Services
{
    public interface IMunicipalityService
    {
        Task<IEnumerable<MunicipalityModel>> GetAllAsync();

        Task<IEnumerable<TaxSupportedMunicipalityModel>> GetTaxSupportingAsync();
    }
}
