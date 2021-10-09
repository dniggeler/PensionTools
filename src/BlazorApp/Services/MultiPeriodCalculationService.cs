using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PensionCoach.Tools.CommonTypes.MultiPeriod;

namespace BlazorApp.Services
{
    public class MultiPeriodCalculationService : IMultiPeriodCalculationService
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;

        public MultiPeriodCalculationService(
            IConfiguration configuration,
            HttpClient httpClient)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
        }

        public async Task<MultiPeriodResponse> CalculateAsync(MultiPeriodRequest request)
        {
            string urlPath = configuration.GetSection("MultiPeriodCalculationServiceUrl").Value;
            var response = await httpClient.GetFromJsonAsync<MultiPeriodResponse>(urlPath);

            return response;
        }
    }
}
