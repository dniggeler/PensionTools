using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Tax.Data;
using Tax.Data.Abstractions.Models;
using TaxCalculator;

namespace Tax.AppConsole
{
    class Program
    {
        static async Task Main()
        {
            var configurationDict = new Dictionary<string, string>
            {
                {"DbSettings:FilePath", @"C:\workspace\private\PensionTools\src\Tax.Data\files\TaxDb"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationDict)
                .Build();

            var provider = GetServiceProvider(configuration);
            
            var logger = provider.GetService<ILogger<Program>>();

            var calculator = provider.GetService<IIncomeTaxCalculator>();
            int calculationYear = 2018;
            var result = await calculator.CalculateAsync(calculationYear, new TaxPerson());

            result
                .Match(
                    Right: r => logger.LogDebug(r.MunicipalityTaxAmount.ToString(CultureInfo.InvariantCulture)),
                    Left: err => logger.LogDebug(err));

            logger.LogError("Hi, error");
        }

        private static IServiceProvider GetServiceProvider(IConfiguration configuration)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTaxData();
            serviceCollection.AddTaxCalculators();
            serviceCollection.AddOptions<DbSettings>();

            serviceCollection.Configure<DbSettings>(configuration.GetSection("DbSettings"));

            serviceCollection.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddConsole();
            });

            return serviceCollection.BuildServiceProvider();
        }
    }
}
