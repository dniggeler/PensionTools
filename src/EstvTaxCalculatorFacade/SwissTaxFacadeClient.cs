using System.Net.Http.Json;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions.Models;

namespace PensionCoach.Tools.EstvTaxCalculators;

public class SwissTaxFacadeClient : ISwissTaxFacadeClient
{
    internal static string SwissTaxFacadeClientName = "SwissTaxFacadeClient";

    private readonly IHttpClientFactory httpClientFactory;

    public SwissTaxFacadeClient(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public async Task<TaxLocation> GetTaxLocationAsync(string zip, string city)
    {
        HttpClient client = httpClientFactory.CreateClient("SwissTaxFacadeClient");
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("ApiKey", "D2787E2D-0A99-491B-AE77-E08EFC1A92F0");
        client.DefaultRequestHeaders.Add("Tenant", "76367A12-A9F6-4C00-819A-3F28A151C787");

        TaxLocation result = await client.GetFromJsonAsync<TaxLocation>($"OrdinaryTax/GetTaxLocation?zip={zip}&city={city}");

        return result;
    }
}
