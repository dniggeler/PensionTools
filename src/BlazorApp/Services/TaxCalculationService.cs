using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.CommonTypes.Tax;

namespace BlazorApp.Services;

public class TaxCalculationService : ITaxCalculationService
{
    private readonly IConfiguration configuration;
    private readonly HttpClient httpClient;
    private readonly ILogger<MockedPensionToolsCalculationService> logger;

    public TaxCalculationService(
        IConfiguration configuration,
        HttpClient httpClient,
        ILogger<MockedPensionToolsCalculationService> logger)
    {
        this.configuration = configuration;
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<FullTaxResponse> CalculateAsync(FullTaxRequest request)
    {
        string baseUri = configuration.GetSection("TaxCalculatorServiceUrl").Value;
        string urlPath = Path.Combine(baseUri, "full/incomeandwealth");

        logger.LogInformation(JsonSerializer.Serialize(request));

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(urlPath, request);

        response.EnsureSuccessStatusCode();

        FullTaxResponse result =
            await response.Content.ReadFromJsonAsync<FullTaxResponse>();

        return result;
    }
}
