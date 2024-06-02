using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp.Services.CheckSettings
{
    public interface ICheckSettingsService
    {
        Task<bool> HealthCheckAsync();

        Task<Dictionary<string, string>> GetFrontendConfigurationAsync();

        Task<Dictionary<string, string>> GetBackendConfigurationAsync();
    }
}
