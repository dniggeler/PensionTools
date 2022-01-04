using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorApp.Services;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using Radzen;

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
                builder.Services.AddScoped<IMunicipalityService, MockedMunicipalityService>();
                builder.Services.AddScoped<IPersonService, MockedPersonService>();
            }
            else
            {
                builder.Services.AddScoped<IMultiPeriodCalculationService, MultiPeriodCalculationService>();
                builder.Services.AddScoped<ITaxCalculationService, TaxCalculationService>();
                builder.Services.AddScoped<IMunicipalityService, MunicipalityServiceClient>();
                builder.Services.AddScoped<IPersonService, PersonService>();
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

            await builder.Build().RunAsync();
        }
    }
}
