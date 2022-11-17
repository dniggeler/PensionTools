using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using Snapshooter.Xunit;
using TaxCalculator.WebApi;
using Xunit;

namespace Calculators.CashFlow.Integration.Tests
{
    [Trait("Cash-Flow", "Integration")]
    public class CashFlowCalculatorsIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient client;

        public CashFlowCalculatorsIntegrationTests()
        {
            var testServer = new TestServer(
                new WebHostBuilder()
                    .ConfigureAppConfiguration((context, builder) =>
                    {
                        builder.AddJsonFile("appsettings.integration.json");
                    })
                    .UseStartup<Startup>());

            client = testServer.CreateClient();
            client.BaseAddress = new Uri("http://localhost/api/calculator/");
        }

        [Fact(DisplayName = "Default")]
        public async Task ShouldCalculateSuccessfully()
        {
            var request = GetRequest();

            var response = await client.PostAsJsonAsync("multiperiod", request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            Snapshot.Match(result);
        }

        private static MultiPeriodRequest GetRequest()
        {
            return new MultiPeriodRequest
            {
                Name = "Test Multi-Period Calculator",
                Gender = Gender.Male,
                DateOfBirth = new DateTime(1969, 3, 17),
                BfsMunicipalityId = 261,
                CivilStatus = CivilStatus.Married,
                ReligiousGroupType = ReligiousGroupType.Other,
                PartnerReligiousGroupType = ReligiousGroupType.Other,
                Income = 100_000,
                Wealth = 500_000,
                CapitalBenefitsPension = 0,
                CapitalBenefitsPillar3A = 0,
                CashFlowDefinitionHolder = new CashFlowDefinitionHolder(),
                StartingYear = 2021,
                NumberOfPeriods = 10
            };
        }
    }
}
