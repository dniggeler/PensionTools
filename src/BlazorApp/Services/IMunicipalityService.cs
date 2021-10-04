using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorApp.MyComponents;

namespace BlazorApp.Services
{
    public interface IMunicipalityService
    {
        Task<IEnumerable<MunicipalityViewModel>> GetAllAsync();
    }
}
