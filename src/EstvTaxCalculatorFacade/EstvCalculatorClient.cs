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
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var result = await client.PostAsync( "API_searchLocation", content);

        if (!result.IsSuccessStatusCode)
        {
            return null;
        }

        result.EnsureSuccessStatusCode();

        string json = await result.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<TaxLocationResponse>(json)?.Response;
    }
}
