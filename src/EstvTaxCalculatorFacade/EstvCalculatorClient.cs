using System.Net.Http.Json;
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

    public async Task<TaxLocation> GetTaxLocationAsync(string zip, string city)
    {
        string request = "{ \"Search\" : ";

        if (string.IsNullOrEmpty(city))
        {
            request += $"\"{zip}\" }}";
        }
        else
        {
            request += $"\"{zip} {city}\" }}";
        }

        HttpClient client = httpClientFactory.CreateClient("EstvTaxCalculatorClient");
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        //client.DefaultRequestHeaders.Add("ApiKey", "D2787E2D-0A99-491B-AE77-E08EFC1A92F0");
        //client.DefaultRequestHeaders.Add("Tenant", "76367A12-A9F6-4C00-819A-3F28A151C787");

        HttpResponseMessage result = await client.PostAsJsonAsync($"API_searchLocation", request);

        return new TaxLocation();
    }
}
