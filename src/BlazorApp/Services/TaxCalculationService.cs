using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using BlazorApp.Services.Mock;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.CommonTypes.Tax;

namespace BlazorApp.Services;

public class TaxCalculationService : ITaxCalculationService, IMarginalTaxCurveCalculationService
{
    private readonly IConfiguration configuration;
    private readonly HttpClient httpClient;
    private readonly ILogger<TaxCalculationService> logger;

    public TaxCalculationService(
        IConfiguration configuration,
        HttpClient httpClient,
        ILogger<TaxCalculationService> logger)
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

    public async Task<int[]> SupportedTaxYearsAsync()
    {
        string baseUri = configuration.GetSection("TaxCalculatorServiceUrl").Value;
        string urlPath = Path.Combine(baseUri, "years");

        return await httpClient.GetFromJsonAsync<int[]>(urlPath);
    }

    public async Task<MarginalTaxResponse> CalculateIncomeCurveAsync(MarginalTaxRequest request)
    {
        string baseUri = configuration.GetSection("TaxCalculatorServiceUrl").Value;
        string urlPath = Path.Combine(baseUri, "marginaltaxcurve/income");

        logger.LogInformation(JsonSerializer.Serialize(request));

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(urlPath, request);

        response.EnsureSuccessStatusCode();

        MarginalTaxResponse result =
            await response.Content.ReadFromJsonAsync<MarginalTaxResponse>();

        return result;

    }
}
