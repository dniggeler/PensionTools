using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace TaxCalculator.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .ConfigureAppConfiguration((context, builder) =>
                        {
                            builder
                                .AddJsonFile("appsettings.json")
                                .AddJsonFile(
                                    $"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
                                    true)
                                .AddEnvironmentVariables()
                                .Build();
                        });
                });
    }
}
