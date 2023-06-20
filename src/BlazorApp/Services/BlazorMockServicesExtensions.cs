﻿using BlazorApp.Services.Mock;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Services;

public static class BlazorMockServicesExtensions
{
    public static IServiceCollection AddMockServices(this IServiceCollection services)
    {
        services.AddScoped<IApexChartConfigurator, ApexChartConfigurator>();
        services.AddScoped<IPersonService, MockedPersonService>();
        
        return services;
    }
}
