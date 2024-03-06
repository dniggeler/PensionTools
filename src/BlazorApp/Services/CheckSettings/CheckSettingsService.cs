using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Services.CheckSettings
{
    public class CheckSettingsService : ICheckSettingsService
    {
        private readonly IHostEnvironment webAssemblyHostEnvironment;
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;
        private readonly ILogger<TaxCalculationService> logger;

        public CheckSettingsService(
            IHostEnvironment webAssemblyHostEnvironment,
            IConfiguration configuration,
            HttpClient httpClient,
            ILogger<TaxCalculationService> logger)
        {
            this.webAssemblyHostEnvironment = webAssemblyHostEnvironment;
            this.configuration = configuration;
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<bool> HealthCheckAsync()
        {
            string urlPath = Path.Combine(BaseAddress(), "health");

            try
            {
                var response = await httpClient.GetStringAsync(urlPath);

                return "Healthy" == response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Health check failed: {ex.Message}");
                return false;
            }
        }

        public Task<Dictionary<string, string>> GetFrontendConfigurationAsync()
        {
            var configs = new Dictionary<string, string>();

            configs.TryAdd("Environment", webAssemblyHostEnvironment.EnvironmentName);
        
            return Task.FromResult(configs);
        }

        public async Task<Dictionary<string, string>> GetBackendConfigurationAsync()
        {
            string urlPath = Path.Combine(BaseAddress(), "api/check/settings");

            try
            {
                return await httpClient.GetFromJsonAsync<Dictionary<string,string>>(urlPath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Backend configuration failed to load: {ex.Message}");
            }

            return new Dictionary<string, string>();
        }

        private string BaseAddress()
        {
            string baseUri = configuration.GetSection("TaxCalculatorServiceUrl").Value;

            if (baseUri is null)
            {
                throw new ArgumentNullException(nameof(baseUri));
            }

            return new Uri(baseUri).GetLeftPart(UriPartial.Authority);
        }
    }
}
