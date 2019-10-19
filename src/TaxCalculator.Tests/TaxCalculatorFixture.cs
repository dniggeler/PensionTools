using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tax.Data;
using Tax.Data.Abstractions.Models;


namespace TaxCalculator.Tests
{
    public class TaxCalculatorFixture<T> 
        where T : class
    {
        public ServiceProvider Provider { get; }

        public T Calculator { get; }

        public TaxCalculatorFixture()
        {
            var configurationDict = new Dictionary<string, string>
            {
                {"DbSettings:FilePath", @"C:\workspace\private\PensionTools\src\Tax.Data\files\TaxDb"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationDict)
                .Build();

            ServiceCollection coll = new ServiceCollection();

            coll.AddTaxData();
            coll.AddTaxCalculators();
            coll.AddOptions<DbSettings>();
            coll.Configure<DbSettings>(configuration.GetSection("DbSettings"));

            Provider = coll.BuildServiceProvider();

            Calculator = Provider.GetRequiredService<T>();
        }
    }
}