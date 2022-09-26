using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
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

        // Serialize our concrete class into a JSON String
        var stringPayload = JsonSerializer.Serialize(request);

        // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
        var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, urlPath)
        {
            Content = httpContent,
        };
        var response = await httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

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
