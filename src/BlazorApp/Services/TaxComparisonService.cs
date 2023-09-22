using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Features.TaxComparison.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.CommonTypes.MultiPeriod;

namespace BlazorApp.Services;

public class TaxComparisonService : ITaxComparisonService
{
    private readonly IConfiguration configuration;
    private readonly HttpClient httpClient;
    private readonly ILogger<TaxComparisonService> logger;

    public TaxComparisonService(
        IConfiguration configuration,
        HttpClient httpClient,
        ILogger<TaxComparisonService> logger)
    {
        this.configuration = configuration;
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async IAsyncEnumerable<TaxComparerResponse> CalculateAsync(CapitalBenefitTaxComparerRequest request)
    {
        await foreach (var result in CalculateAsync(request, "capitalbenefit"))
        {
            yield return result;
        }
    }

    public async IAsyncEnumerable<TaxComparerResponse> CalculateAsync(IncomeAndWealthComparerRequest request)
    {
        await foreach (var result in CalculateAsync(request, "incomewealth"))
        {
            yield return result;
        }
    }

    public Task<MultiPeriodResponse> CalculateAsync(CapitalBenefitTransferInComparerRequest request)
    {
        throw new System.NotImplementedException();
    }

    private async IAsyncEnumerable<TaxComparerResponse> CalculateAsync<T>(T request, string urlMethodPart)
    {
        string baseUri = configuration.GetSection("TaxComparisonServiceUrl").Value;
        string urlPath = Path.Combine(baseUri, urlMethodPart);

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
            .ReadFromJsonAsync<IAsyncEnumerable<TaxComparerResponse>>();

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
