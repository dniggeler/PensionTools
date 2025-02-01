using Application.Tax.Estv.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.EstvTaxCalculator;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEstvTaxCalculatorClient(this IServiceCollection services, IConfiguration configuration)
    {
        string baseUrl = configuration["TaxCalculatorClient:EstvTaxCalculatorBaseUrl"];
        var useCache = configuration.GetSection("TaxCalculatorClient").GetValue<bool>("UseCache");

        return services.AddEstvTaxCalculatorClient(baseUrl, useCache);
    }

    public static IServiceCollection AddEstvTaxCalculatorClient(this IServiceCollection services, string baseUrl, bool useCache)
    {
        services.AddHttpClient(EstvTaxCalculatorClientBase.EstvTaxCalculatorClientName, c =>
        {
            c.BaseAddress = new Uri(baseUrl);
        });

        services
            .AddTransient<CachedEstvTaxCalculatorClient>()
            .AddTransient<EstvTaxCalculatorClient>()
            .AddTransient<IEstvTaxCalculatorClient>(provider =>
            {
                if (useCache)
                {
                    return provider.GetRequiredService<CachedEstvTaxCalculatorClient>();
                }

                return provider.GetRequiredService<EstvTaxCalculatorClient>(); ;
            });

        return services;
    }
}
