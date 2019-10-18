using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Tax.Data;
using Tax.Data.Abstractions.Models;
using TaxCalculator;

namespace Tax.AppConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configurationDict = new Dictionary<string, string>
            {
                {"DbSettings:FilePath", @"C:\workspace\private\PensionTools\src\Tax.Data\files\TaxDb"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationDict)
                .Build();

            var provider = GetServiceProvider(configuration);

            var calculator = provider.GetService<IIncomeTaxCalculator>();
            var result = await calculator.CalculateAsync(new TaxPerson());
        }

        private static IServiceProvider GetServiceProvider(IConfiguration configuration)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTaxData();
            serviceCollection.AddTaxCalculators();
            serviceCollection.AddOptions<DbSettings>();

            serviceCollection.Configure<DbSettings>(configuration.GetSection("DbSettings"));



            return serviceCollection.BuildServiceProvider();
        }
    }
}
