using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Application.Tax.Proprietary.Models;
using Domain.Enums;
using Domain.Models.Tax;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using PensionCoach.Tools.CommonTypes.Tax;
using Snapshooter.Xunit;
using TaxCalculator.WebApi;
using Xunit;

namespace TaxCalculator.Integration.Tests;

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
                    // add memory collection
                    builder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"ApplicationMode", "Estv"},
                    });

                    builder.AddJsonFile("appsettings.integration.json", false);
                })
                .UseStartup<Startup>());

        client = testServer.CreateClient();
        client.BaseAddress = new Uri("http://localhost/api/calculators/tax/");
    }

    [Fact(DisplayName = "Full Tax Supported Municipalities")]
    public async Task Get_Full_Tax_Supported_Municipalities()
    {
        // given

        // when
        var result = await client.GetFromJsonAsync<IEnumerable<TaxSupportedMunicipalityModel>>("municipality");

        // then
        Snapshot.Match(result);
    }

    [Fact(DisplayName = "Marginal Tax Curve for Capital Benefits")]
    public async Task Get_Marginal_Tax_Curve_For_Capital_Benefits()
    {
        // given
        var request = new
        {
            Name = "Marginal Tax Curve",
            CalculationYear = 2023,
            CivilStatus = CivilStatus.Married,
            ReligiousGroup = ReligiousGroupType.Protestant,
            PartnerReligiousGroup = ReligiousGroupType.Protestant,
            BfsMunicipalityId = 2526,
            TaxableAmount = 810_000,
            LowerSalaryLimit = 0,
            UpperSalaryLimit = 1_000_000,
            numberOfSamples = 25
        };

        // when
        var response = await client.PostAsJsonAsync("marginaltaxcurve/capitalbenefits", request);

        // then
        var result = response.EnsureSuccessStatusCode();

        var content = await result.Content.ReadFromJsonAsync<MarginalTaxCurveResult>();

        Snapshot.Match(content);
    }
}
