using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Features.TaxScenarios.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.CommonTypes.Features.PensionVersusCapital;
using PensionCoach.Tools.CommonTypes.MultiPeriod;

namespace BlazorApp.Services;

public class TaxScenarioService : ITaxScenarioService
{
    private readonly IConfiguration configuration;
    private readonly HttpClient httpClient;
    private readonly ILogger<TaxComparisonService> logger;

    public TaxScenarioService(
        IConfiguration configuration,
        HttpClient httpClient,
        ILogger<TaxComparisonService> logger)
    {
        this.configuration = configuration;
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public Task<ScenarioCalculationResponse> CalculateAsync(CapitalBenefitTransferInComparerRequest request)
    {
        return CalculateAsync(request, "CalculateCapitalBenefitTransferInsYears");
    }

    public Task<ScenarioCalculationResponse> CalculateAsync(PensionVersusCapitalRequest request)
    {
        return CalculateAsync(request, "CalculatePensionVersusCapitalComparison");
    }

    private async Task<ScenarioCalculationResponse> CalculateAsync<T>(T request, string urlMethodPart)
    {
        string baseUri = configuration.GetSection("TaxScenarioServiceUrl").Value;
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

        return await response.Content.ReadFromJsonAsync<ScenarioCalculationResponse>();
    }
}
