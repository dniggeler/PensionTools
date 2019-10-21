using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tax.Data.Abstractions.Models;

namespace Tax.Data.Tests
{
    public class TaxDataFixture
    {
        public ServiceProvider Provider { get; }

        public TaxDataFixture()
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

            ServiceCollection coll = new ServiceCollection();

            coll.AddTaxData();
            coll.AddOptions<DbSettings>();
            coll.Configure<DbSettings>(configuration.GetSection("DbSettings"));

            Provider = coll.BuildServiceProvider();
        }
    }
}