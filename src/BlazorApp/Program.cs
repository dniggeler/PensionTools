using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorApp.Services;
using Radzen;

namespace BlazorApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            bool isMocked = Convert.ToBoolean(builder.Configuration.GetSection("isMocked").Value);

            if (isMocked)
            {
                builder.Services.AddScoped<IMultiPeriodCalculationService, MockedMultiPeriodCalculationService>();
                builder.Services.AddScoped<IMunicipalityService, MockedMunicipalityService>();
            }
            else
            {
                builder.Services.AddScoped<IMultiPeriodCalculationService, MultiPeriodCalculationService>();
                builder.Services.AddScoped<IMunicipalityService, MunicipalityServiceClient>();
            }

            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<TooltipService>();
            builder.Services.AddScoped<ContextMenuService>();

            await builder.Build().RunAsync();
        }
    }
}
