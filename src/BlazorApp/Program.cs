﻿using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorApp.Services;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using Radzen;
using BlazorApp.Services.Mock;
using System.Globalization;

namespace BlazorApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            if (builder.HostEnvironment.IsEnvironment("Mock"))
            {
                builder.Services.AddScoped<IMultiPeriodCalculationService, MockedPensionToolsCalculationService>();
                builder.Services.AddScoped<ITaxCalculationService, MockedPensionToolsCalculationService>();
                builder.Services.AddScoped<IMarginalTaxCurveCalculationService, MockedPensionToolsCalculationService>();
                builder.Services.AddScoped<ITaxComparisonService, MockTaxComparisonService>();
                builder.Services.AddScoped<ITaxScenarioService, MockTaxComparisonService>();
                builder.Services.AddScoped<IMunicipalityService, MockedMunicipalityService>();
                builder.Services.AddScoped<IPersonService, MockedPersonService>();
                builder.Services.AddScoped<IExportService, ExportService>();
            }
            else
            {
                builder.Services.AddScoped<IMultiPeriodCalculationService, MultiPeriodCalculationService>();
                builder.Services.AddScoped<ITaxCalculationService, TaxCalculationService>();
                builder.Services.AddScoped<IMarginalTaxCurveCalculationService, TaxCalculationService>();
                builder.Services.AddScoped<ITaxComparisonService, TaxComparisonService>();
                builder.Services.AddScoped<IMunicipalityService, MunicipalityServiceClient>();
                builder.Services.AddScoped<ITaxScenarioService, MockTaxComparisonService>();
                builder.Services.AddScoped<IPersonService, PersonService>();
                builder.Services.AddScoped<IExportService, ExportService>();
            }

            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<TooltipService>();
            builder.Services.AddScoped<ContextMenuService>();

            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddLocalization();
            
            if (!builder.HostEnvironment.IsProduction())
            {
                builder.Services.AddLogging(b => b.SetMinimumLevel(LogLevel.Debug).AddFilter("Microsoft", LogLevel.Information));
            }

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("de-CH");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("de-CH");

            await builder.Build().RunAsync();
        }
    }
}
