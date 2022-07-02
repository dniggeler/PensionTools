using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Municipality;
using Snapshooter.Xunit;
using TaxCalculator.WebApi;
using Xunit;
using Xunit.Abstractions;

namespace Tax.Data.Integration.Tests
{
    [Trait("Municipality", "Integration")]
    public class MunicipalityDataIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly ITestOutputHelper outputHelper;
        private readonly HttpClient client;

        public MunicipalityDataIntegrationTests(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
            var testServer = new TestServer(
                new WebHostBuilder()
                    .ConfigureAppConfiguration((_, builder) =>
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

            var result = await response.Content.ReadFromJsonAsync<IEnumerable<MunicipalityModel>>();

            Snapshot.Match(result);
        }

        [Fact(DisplayName = "Populate With Zip Codes")]
        public async Task Populate_With_All_ZipCodes()
        {
            HttpResponseMessage response = await client.PostAsync($"zip/populate", null);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<int>();
            
            Assert.True(result > 0);
        }

        [Fact(DisplayName = "Stage Zip Codes")]
        public async Task Stage_With_ZipCodes()
        {
            HttpResponseMessage response = await client.PostAsync("zip/stage", null);

            response.EnsureSuccessStatusCode();

            int result = await response.Content.ReadFromJsonAsync<int>();

            Assert.True(result > 0);
        }

        [Fact(DisplayName = "Populate With Tax Location")]
        public async Task Populate_With_Tax_Location()
        {
            bool doClear = false;

            HttpResponseMessage response = await client.PostAsync($"tax/populate/{doClear}", null);

            response.EnsureSuccessStatusCode();

            int result = await response.Content.ReadFromJsonAsync<int>();

            outputHelper.WriteLine($"Number of updates: {result}");

            Assert.True(result > 0);
        }

        [Fact(DisplayName = "Clean Municipality Names")]
        public async Task Clean_Municipality_Names()
        {
            HttpResponseMessage response = await client.PostAsync($"tax/clean", null);

            response.EnsureSuccessStatusCode();

            int result = await response.Content.ReadFromJsonAsync<int>();

            Assert.True(result > 0);
        }

        private static MunicipalitySearchFilter GetRequest()
        {
            return new MunicipalitySearchFilter
            {
                Name = "Z",
                Canton = Canton.ZH,
                YearOfValidity = 2021
            };
        }
    }
}
