using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorApp.Services.CheckSettings;

namespace BlazorApp.Services.Mock
{
    public class MockedCheckSettingsCheck : ICheckSettingsService
    {
        public Task<bool> HealthCheckAsync()
        {
            return Task.FromResult(true);
        }

        public Task<Dictionary<string, string>> GetFrontendConfigurationAsync()
        {
            return Task.FromResult(new Dictionary<string, string>()
            {
                {"Environment", "Mocked"}
            });
        }

        public Task<Dictionary<string, string>> GetBackendConfigurationAsync()
        {
            return Task.FromResult(new Dictionary<string, string>()
            {
                {"Steuerrechner", "Mock"}
            });
        }
    }
}
