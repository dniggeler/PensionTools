using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions.Models;
using Snapshooter.Xunit;
using TaxCalculator.WebApi;
using Xunit;

namespace EstvTaxCalculatorFacade.Integration.Tests;

[Trait("Tax Calculator Clients", "Integration")]
public class TaxCalculatorClientIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly HttpClient estvClient;

    public TaxCalculatorClientIntegrationTests(WebApplicationFactory<Startup> factory)
    {
        estvClient = factory.CreateDefaultClient(new Uri("http://localhost/api/calculators/tax/"));
    }

    [Fact(DisplayName = "Tax Location by ESTV")]
    public async Task Should_Get_TaxLocation_By_Estv_Successfully()
    {
        string zip = "3303";
        string city = "Zuzwil";

        var result = await estvClient.GetFromJsonAsync<TaxLocation[]>($"location?zip={zip}&city={city}");

        Snapshot.Match(result);
    }
}
