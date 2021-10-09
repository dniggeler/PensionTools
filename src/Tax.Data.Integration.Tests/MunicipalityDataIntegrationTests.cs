using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Municipality;
using Snapshooter.Xunit;
using TaxCalculator.WebApi;
using Xunit;

namespace Tax.Data.Integration.Tests
{
    [Trait("Municipality", "Integration")]
    public class MunicipalityDataIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient client;

        public MunicipalityDataIntegrationTests()
        {
            var testServer = new TestServer(
                new WebHostBuilder()
                    .ConfigureAppConfiguration((context, builder) =>
                    {
                        builder.AddJsonFile("appsettings.integration.json");
                    })
                    .UseStartup<Startup>());

            client = testServer.CreateClient();
            client.BaseAddress = new Uri("http://localhost/api/data/municipality/");
        }

        [Fact(DisplayName = "Search")]
        public async Task ShouldCalculateSuccessfully()
        {
            var request = GetRequest();

            var response = await client.PostAsJsonAsync("search", request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            Snapshot.Match(result);
        }

        private static MunicipalitySearchFilter GetRequest()
        {
            return new MunicipalitySearchFilter
            {
                Name = "Z",
                Canton = Canton.ZH,
                YearOfValidity = 2019
            };
        }
    }
}
