using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using PensionCoach.Tools.CommonTypes.Tax;
using Snapshooter.Xunit;
using TaxCalculator.WebApi;
using Xunit;

namespace TaxCalculator.Integration.Tests
{
    [Trait("Tax Calculator", "Integration")]
    public class TaxCalculatorIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient client;

        public TaxCalculatorIntegrationTests()
        {
            var testServer = new TestServer(
                new WebHostBuilder()
                    .ConfigureAppConfiguration((_, builder) =>
                    {
                        builder.AddJsonFile("appsettings.integration.json");
                    })
                    .UseStartup<Startup>());

            client = testServer.CreateClient();
            client.BaseAddress = new Uri("http://localhost/api/calculators/tax/");
        }

        [Fact(DisplayName = "Full Tax Supported Municipalities")]
        public async Task Get_Full_Tax_Supported_Municipalities()
        {
            // given
            int year = 2018;

            var result = await client.GetFromJsonAsync<IEnumerable<TaxSupportedMunicipalityModel>>($"municipality/{year}");

            Snapshot.Match(result);
        }
    }
}
