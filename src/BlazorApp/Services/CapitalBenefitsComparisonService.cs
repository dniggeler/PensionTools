using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.TaxComparison;

namespace BlazorApp.Services;

public class CapitalBenefitsComparisonService : ITaxCapitalBenefitsComparisonService
{
    private readonly IConfiguration configuration;
    private readonly HttpClient httpClient;
    private readonly ILogger<CapitalBenefitsComparisonService> logger;

    public CapitalBenefitsComparisonService(
        IConfiguration configuration,
        HttpClient httpClient,
        ILogger<CapitalBenefitsComparisonService> logger)
    {
        this.configuration = configuration;
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async IAsyncEnumerable<CapitalBenefitTaxComparerResponse> CalculateAsync(CapitalBenefitTaxComparerRequest request)
    {
        string baseUri = configuration.GetSection("TaxComparisonServiceUrl").Value;
        string urlPath = Path.Combine(baseUri, "capitalbenefit");

        logger.LogInformation(JsonSerializer.Serialize(request));

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(urlPath, request);

        response.EnsureSuccessStatusCode();

        var asyncResponse = await response.Content
            .ReadFromJsonAsync<IAsyncEnumerable<CapitalBenefitTaxComparerResponse>>();

        if (asyncResponse is null)
        {
            yield break;
        }

        await foreach (var comparisonResult in asyncResponse)
        {
            yield return comparisonResult;
        }
    }
}
