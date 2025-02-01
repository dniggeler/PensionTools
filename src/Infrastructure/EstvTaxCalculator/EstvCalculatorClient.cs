using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Infrastructure.EstvTaxCalculator;

public class EstvTaxCalculatorClient(IHttpClientFactory httpClientFactory, ILogger<EstvTaxCalculatorClient> logger)
    : EstvTaxCalculatorClientBase
{
        
    protected override async Task<TOut> CallAsync<TOut>(string request, string path)
    {
        using HttpClient client = httpClientFactory.CreateClient(EstvTaxCalculatorClientName);

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        var content = new StringContent(request, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(path, content);

        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<TOut>(json);
    }
}
