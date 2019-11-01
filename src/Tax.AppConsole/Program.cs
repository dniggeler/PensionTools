using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
            var projectPath = Assembly.GetExecutingAssembly()
                .Location.Split("src", StringSplitOptions.RemoveEmptyEntries)
                .First();

            var dbFile = Path.Combine(projectPath, @"src\Tax.Data\files\TaxDb");


            var configurationDict = new Dictionary<string, string>
            {
                {"DbSettings:FilePath", dbFile}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationDict)
                .Build();

            var provider = GetServiceProvider(configuration);
            
            var logger = provider.GetService<ILogger<Program>>();

            var calculator = provider.GetService<IFullTaxCalculator>();
            int calculationYear = 2018;
            var taxPerson = new TaxPerson
            {
                Canton = "ZH",
                Name = "Burli",
                CivilStatus = CivilStatus.Married,
                DenominationType = ReligiousGroupType.Married,
                Municipality = "Zürich",
                TaxableIncome = 125000,
                TaxableFederalIncome = 125000,
                TaxableWealth = 300000
            };
            var result = await calculator.CalculateAsync(calculationYear, taxPerson);

            result
                .Match(
                    Right: r => logger.LogInformation(r.TotalTaxAmount.ToString(CultureInfo.InvariantCulture)),
                    Left: err => logger.LogInformation(err));
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
