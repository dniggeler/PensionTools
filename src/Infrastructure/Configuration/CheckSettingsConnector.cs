using Application.Features.CheckSettings;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Configuration
{
    public class CheckSettingsConnector : ICheckSettingsConnector
    {
        private readonly IConfiguration _configuration;

        public CheckSettingsConnector(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<Dictionary<string, string>> GetAsync()
        {
            _configuration.GetApplicationMode();

            var settings = new Dictionary<string, string> { { "Steuerrechner", _configuration.GetApplicationMode().ToString() } };

            return Task.FromResult(settings);
        }
    }
}
