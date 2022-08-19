using Microsoft.Extensions.DependencyInjection;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions;

namespace PensionCoach.Tools.EstvTaxCalculators;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEstvTaxCalculatorClient(this IServiceCollection services, string baseUrl)
    {
        services.AddHttpClient(EstvTaxCalculatorClient.EstvTaxCalculatorClientName, c =>
        {
            c.BaseAddress = new Uri(baseUrl);
        });

        services.AddTransient<IEstvTaxCalculatorClient, EstvTaxCalculatorClient>();

        return services;
    }
}
