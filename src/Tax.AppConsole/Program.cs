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
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
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

            var calculator = provider.GetService<ICapitalBenefitTaxCalculator>();
            int calculationYear = 2018;
            int municipalityId = 261;
            Canton canton = Canton.ZH;

            decimal[] incomes = {0M, 1000, 10000, 20000, 40000, 70000, 90000, 99995, 120000,150000,200000,300000,500000,800000,1000000,1500000,2000000,4000000};

            Parallel.ForEach(incomes, async amount =>
            {
                var taxPerson = new CapitalBenefitTaxPerson
                {
                    Name = "Burli",
                    CivilStatus = CivilStatus.Single,
                    ReligiousGroupType = ReligiousGroupType.Protestant,
                    TaxableBenefits = amount,
                };

                var r = await calculator.CalculateAsync(
                    calculationYear, municipalityId, canton, taxPerson);
                Console.WriteLine(
                    $"Benefits: {amount}, Total tax: {r.IfRight(v => v.TotalTaxAmount.ToString(CultureInfo.InvariantCulture))}");
            });
        }

        private static IServiceProvider GetServiceProvider(IConfiguration configuration)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTaxData(configuration);
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
