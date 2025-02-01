using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Replicant;

namespace Infrastructure.EstvTaxCalculator;

public class CachedEstvTaxCalculatorClient(IHttpClientFactory httpClientFactory, ILogger<EstvTaxCalculatorClient> logger)
    : EstvTaxCalculatorClientBase
{
    private const string CacheDirectory = "/temp/replicant";

    protected override async Task<TOut> CallAsync<TOut>(string request, string path)
    {
        using HttpClient client = httpClientFactory.CreateClient(EstvTaxCalculatorClientName);

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        var content = new StringContent(request, Encoding.UTF8, "application/json");

        HttpCache cache = new HttpCache(CacheDirectory, client, 10000);

        string hashRequest = Hash.Compute(request);

        string realUri = $"{client.BaseAddress?.AbsoluteUri}{path}";
        string cacheUri = $"{realUri}?tag={hashRequest}";
        bool cacheHit = true;
        HttpResponseMessage response = await cache.ResponseAsync(
            cacheUri, true, m =>
            {
                cacheHit = false;
                m.Method = HttpMethod.Post;
                m.RequestUri = new Uri(realUri);
                m.Content = content;
            }, CancellationToken.None);

        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();

        if (cacheHit is false)
        {
            await cache.AddItemAsync(cacheUri, json, null, null, null, null, null, null, CancellationToken.None);
        }
        else
        {
            logger.LogInformation("Cache hit for ESTV calculator");
        }

        return JsonSerializer.Deserialize<TOut>(json);
    }
}
