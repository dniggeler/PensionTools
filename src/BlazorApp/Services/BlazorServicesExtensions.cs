using BlazorApp.Services.CheckSettings;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Services
{
    public static class BlazorServicesExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IApexChartConfigurator, ApexChartConfigurator>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<ICheckSettingsService, CheckSettingsService>();

            return services;
        }
    }
}
