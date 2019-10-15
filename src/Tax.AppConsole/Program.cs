using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tax.Data;
using Tax.Data.Abstractions.Models;

namespace Tax.AppConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var configurationDict = new Dictionary<string, string>
            {
                {"DbSettings:FilePath", @"C:\workspace\private\PensionTools\src\Tax.Data\files\TaxDb"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationDict)
                .Build();

            var provider = GetServiceProvider(configuration);

            using (var dbContext = provider.GetService<TaxTariffDbContext>())
            {
                foreach (var taxItem in dbContext.Tariffs.Where(item => item.Canton == "ZH" && 
                                                                        item.Year == 2018))
                {
                    Console.WriteLine($"{taxItem.IncomeLevel}: {taxItem.TaxAmount}");
                }
            }
        }

        private static IServiceProvider GetServiceProvider(IConfiguration configuration)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDbContext<TaxTariffDbContext>();
            serviceCollection.AddDbContext<TaxRateDbContext>();
            serviceCollection.AddOptions<DbSettings>();

            serviceCollection.Configure<DbSettings>(configuration.GetSection("DbSettings"));

            return serviceCollection.BuildServiceProvider();
        }
    }
}
