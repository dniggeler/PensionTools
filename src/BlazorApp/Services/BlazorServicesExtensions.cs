using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Services;

public static class BlazorServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPersonService, PersonService>();

        return services;
    }
}
