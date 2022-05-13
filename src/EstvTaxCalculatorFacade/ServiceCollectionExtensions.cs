using Microsoft.Extensions.DependencyInjection;

namespace PensionCoach.Tools.EstvTaxCalculators;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEstvClient(this IServiceCollection services, string baseUrl)
    {
        services.AddHttpClient("EstvClient", c =>
        {
            c.BaseAddress = new Uri(baseUrl);
        });

        services.AddTransient<IEstvFacadeClient, EstvFacadeClient>();

        return services;
    }
}
