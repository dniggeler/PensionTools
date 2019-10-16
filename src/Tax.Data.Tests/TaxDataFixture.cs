using System.Collections.Generic;
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
            var configurationDict = new Dictionary<string, string>
            {
                {"DbSettings:FilePath", @"C:\workspace\private\PensionTools\src\Tax.Data\files\TaxDb"}
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