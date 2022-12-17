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
using PensionCoach.Tools.TaxComparison;
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
                    .ConfigureAppConfiguration((_, builder) =>
                    {
                        builder.AddJsonFile("appsettings.integration.json");
                    })
                    .UseStartup<Startup>());

            client = testServer.CreateClient();
            client.BaseAddress = new Uri("http://localhost/api/scenario/tax/");
        }

        [Fact(DisplayName = "Default")]
        public async Task ShouldCalculateSuccessfully()
        {
            CapitalBenefitTransferInComparerRequest request = GetRequest();

            var response = await client.PostAsJsonAsync("CalculateTransferInCapitalBenefits", request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            Snapshot.Match(result);
        }

        private static CapitalBenefitTransferInComparerRequest GetRequest()
        {
            int yearOfCapitalBenefitWithdrawal = 2030;

            return new CapitalBenefitTransferInComparerRequest
            {
                Name = "Test Multi-Period Calculator",
                CalculationYear = 2021,
                BfsMunicipalityId = 261,
                CivilStatus = CivilStatus.Married,
                ReligiousGroup = ReligiousGroupType.Other,
                PartnerReligiousGroup = ReligiousGroupType.Other,
                TaxableIncome = 100_000,
                TaxableFederalIncome = 100_000,
                TaxableWealth = 500_000,
                TransferIns = new List<SingleTransferInModel>
                {
                    new(10_000, new DateTime(2021, 1, 1))
                },
                WithCapitalBenefitTaxation = true,
                Withdrawals = new List<SingleTransferInModel>
                {
                    new(500_000, new DateTime(yearOfCapitalBenefitWithdrawal, 1, 1))
                },
            };
        }
    }
}
