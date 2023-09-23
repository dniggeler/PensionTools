using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Application.Features.FullTaxCalculation;
using Application.Features.TaxComparison;
using Infrastructure.Configuration;
using Infrastructure.Tax.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Tax.Tools.Comparison.Tests
{
    public class TaxComparerFixture<T>
        where T : class
    {
        public ServiceProvider Provider { get; }

        public T Calculator { get; }

        public T Service { get; }

        public TaxComparerFixture()
        {
            var projectPath = Assembly.GetExecutingAssembly()
                .Location.Split("src", StringSplitOptions.RemoveEmptyEntries)
                .First();

            var dbFile = Path.Combine(projectPath, @"src\Infrastructure\files\TaxDb.db");

            var configurationDict = new Dictionary<string, string>
            {
                {"ConnectionStrings:TaxDb", dbFile},
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationDict)
                .Build();

            ServiceCollection coll = new ServiceCollection();

            coll.AddLogging();
            coll.AddTaxCalculators(configuration.GetApplicationMode());
            coll.AddTaxData(configuration);
            coll.AddTaxComparers();

            Provider = coll.BuildServiceProvider();

            Calculator = Provider.GetRequiredService<T>();

            Service = Provider.GetRequiredService<T>();
        }
    }
}
