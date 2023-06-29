using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Services;

public class HealthCheckService : IHealthCheckService
{
    private readonly IConfiguration configuration;
    private readonly HttpClient httpClient;
    private readonly ILogger<TaxCalculationService> logger;

    public HealthCheckService(
        IConfiguration configuration,
        HttpClient httpClient,
        ILogger<TaxCalculationService> logger)
    {
        this.configuration = configuration;
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public Task<bool> CheckAsync()
    {
        return Task.FromResult(true);
    }
}
