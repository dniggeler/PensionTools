using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions.Models;
using PensionCoach.Tools.EstvTaxCalculators.Models;

namespace PensionCoach.Tools.EstvTaxCalculators;

public class EstvTaxCalculatorClient : IEstvTaxCalculatorClient
{
    internal static string EstvTaxCalculatorClientName = "EstvTaxCalculatorClient";

    private readonly IHttpClientFactory httpClientFactory;

    public EstvTaxCalculatorClient(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public async Task<TaxLocation[]> GetTaxLocationsAsync(string zip, string city)
    {
        HttpClient client = httpClientFactory.CreateClient("EstvTaxCalculatorClient");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        var request = new TaxLocationRequest { Search = string.IsNullOrEmpty(city) ? $"{zip}" : $"{zip} {city}" };

        TaxLocationResponse response = await CallAsync<TaxLocationResponse>(JsonSerializer.Serialize(request), "API_searchLocation");

        return response.Response;
    }

    public async Task<SimpleTaxResult> CalculateIncomeAndWealthTaxAsync(SimpleTaxRequest request)
    {
        SimpleTaxResponse response = await CallAsync<SimpleTaxResponse>(JsonSerializer.Serialize(request), "API_calculateSimpleTaxes");

        return response.Response;    
    }

    private async Task<TOut> CallAsync<TOut>(string request, string path)
    {
        HttpClient client = httpClientFactory.CreateClient("EstvTaxCalculatorClient");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        var content = new StringContent(request, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync(path, content);
        
        response.EnsureSuccessStatusCode();
        
        string json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<TOut>(json);
    }
}
