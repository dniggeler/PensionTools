using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.CommonTypes.MultiPeriod;

namespace BlazorApp.Services
{
    public class MockedMultiPeriodCalculationService : IMultiPeriodCalculationService
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;
        private readonly ILogger<MockedMultiPeriodCalculationService> logger;

        public MockedMultiPeriodCalculationService(
            IConfiguration configuration,
            HttpClient httpClient,
            ILogger<MockedMultiPeriodCalculationService> logger)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<MultiPeriodResponse> CalculateAsync(MultiPeriodRequest request)
        {
            string urlPath = configuration.GetSection("MultiPeriodCalculationServiceUrl").Value;
            var response = await httpClient.GetFromJsonAsync<MultiPeriodResponse>(urlPath);

            return response;
        }
    }
}
