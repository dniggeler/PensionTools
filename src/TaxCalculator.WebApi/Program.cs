using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                    var projectPath = Assembly.GetExecutingAssembly()
                        .Location.Split("src", StringSplitOptions.RemoveEmptyEntries)
                        .First();

                    var dbFile = Path.Combine(projectPath, @"/app/data/TaxDb.db");

                    var configurationDict = new Dictionary<string, string>
                    {
                        {"ConnectionStrings:TaxDb", dbFile}
                    };

                    webBuilder.UseStartup<Startup>()
                        .ConfigureAppConfiguration((context, builder) =>
                        {
                            builder
                                .AddInMemoryCollection(configurationDict)
                                .Build();
                        });
                });
    }
}
