using System.Collections.Generic;
using System.Threading.Tasks;
using PensionCoach.Tools.CommonTypes;

namespace BlazorApp.Services
{
    public interface IMunicipalityService
    {
        Task<IEnumerable<MunicipalityModel>> GetAllAsync();
    }
}
