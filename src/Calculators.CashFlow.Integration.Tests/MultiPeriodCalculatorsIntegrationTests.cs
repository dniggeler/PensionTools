using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;
using Snapshooter.Xunit;
using TaxCalculator.WebApi;
using Xunit;

namespace Calculators.CashFlow.Integration.Tests;

[Trait("Cash-Flow", "Integration")]
public class MultiPeriodCalculatorsIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly HttpClient client;

    public MultiPeriodCalculatorsIntegrationTests()
    {
        var testServer = new TestServer(
            new WebHostBuilder()
                .ConfigureAppConfiguration((_, builder) =>
                {
                    builder.AddJsonFile("appsettings.integration.json");
                })
                .UseStartup<Startup>());

        client = testServer.CreateClient();
        client.BaseAddress = new Uri("http://localhost/api/calculator/");
    }

    [Fact(DisplayName = "Wealth Only")]
    public async Task Calculate_Wealth_Only()
    {
        MultiPeriodRequest request = GetRequest();

        var response = await client.PostAsJsonAsync("multiperiod", request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        Snapshot.Match(result);
    }

    [Fact(DisplayName = "Wealth And Income")]
    public async Task Calculate_Wealth_And_Income()
    {
        // given
        decimal income = 100000;

        MultiPeriodRequest request = GetRequest();

        request.CashFlowDefinitionRequest.SalaryPaymentsDefinition = new SalaryPaymentsDefinition
        {
            YearlyAmount = income,
            DateOfEndOfPeriod = new DateTime(2034, 4, 1),
        };

        var response = await client.PostAsJsonAsync("multiperiod", request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        Snapshot.Match(result);
    }

    private static MultiPeriodRequest GetRequest()
    {
        decimal wealth = 500000;

        return new MultiPeriodRequest
        {
            StartingYear = 2023,
            NumberOfPeriods = 10,
            Name = "Test Multi-Period Calculator",
            BfsMunicipalityId = 261,
            CivilStatus = CivilStatus.Single,
            Income = 0,
            Wealth = wealth,
            DateOfBirth = new DateTime(1980, 1, 1),
            ReligiousGroupType = ReligiousGroupType.Other,
            Gender = Gender.Male,
            CashFlowDefinitionRequest = new()
            {
                SetupAccountDefinition = new SetupAccountDefinition
                {
                    InitialWealth = wealth
                }
            }
        };
    }
}
