using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.Tax;

namespace BlazorApp.Services
{
    public class MockedPensionToolsCalculationService : IMultiPeriodCalculationService, ITaxCalculationService
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;
        private readonly ILogger<MockedPensionToolsCalculationService> logger;

        public MockedPensionToolsCalculationService(
            IConfiguration configuration,
            HttpClient httpClient,
            ILogger<MockedPensionToolsCalculationService> logger)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<MultiPeriodResponse> CalculateAsync(MultiPeriodRequest request)
        {
            string urlPath = configuration.GetSection("MultiPeriodCalculationServiceUrl").Value;
            foreach (var keyValuePair in configuration.AsEnumerable())
            {
                logger.LogInformation($"{keyValuePair.Key}: {keyValuePair.Value}");
            }
            var response = await httpClient.GetFromJsonAsync<MultiPeriodResponse>(urlPath);

            return response;
        }

        public Task<FullTaxResponse> CalculateAsync(FullTaxRequest request)
        {
            if (request.BfsMunicipalityId == 0)
            {
                throw new ArgumentException(nameof(request.BfsMunicipalityId));
            }

            logger.LogInformation(JsonSerializer.Serialize(request));

            FullTaxResponse taxCalculationResponse = new()
            {
                Name = request.Name,
                CalculationYear = request.CalculationYear,
                CantonTaxAmount = 1000,
                ChurchTaxAmount = 100,
                FederalTaxAmount = 500,
                MunicipalityTaxAmount = 1500,
                PollTaxAmount = 5,
                WealthTaxAmount = 200,
                TotalTaxAmount = 3200,
                TaxRateDetails = new TaxRateDetails()
                {
                    CantonRate = 1M,
                    ChurchTaxRate = 0.02M,
                    MunicipalityRate = 1.19M
                }
            };

            return Task.FromResult(taxCalculationResponse);
        }
    }
}
