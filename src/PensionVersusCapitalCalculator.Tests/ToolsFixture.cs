using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tax.Data;
using TaxCalculator;

namespace PensionVersusCapitalCalculator.Tests
{
    public class ToolsFixture<T>
        where T : class
    {
        public ServiceProvider Provider { get; }

        public T Calculator { get; }

        public T Service { get; }

        public ToolsFixture()
        {
            var projectPath = Assembly.GetExecutingAssembly()
                .Location.Split("src", StringSplitOptions.RemoveEmptyEntries)
                .First();

            var dbFile = Path.Combine(projectPath, @"src\Tax.Data\files\TaxDb.db");

            var configurationDict = new Dictionary<string, string>
            {
                {"ConnectionStrings:TaxDb", dbFile}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationDict)
                .Build();

            ServiceCollection coll = new ServiceCollection();
            coll.AddToolsCalculators();
            coll.AddTaxCalculators();
            coll.AddTaxData(configuration);

            Provider = coll.BuildServiceProvider();

            Calculator = Provider.GetRequiredService<T>();

            Service = Provider.GetRequiredService<T>();
        }
    }
}
