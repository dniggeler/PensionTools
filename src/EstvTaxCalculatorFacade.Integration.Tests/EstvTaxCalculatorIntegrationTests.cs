using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using PensionCoach.Tools.EstvTaxCalculators.Models;
using Snapshooter.Xunit;
using TaxCalculator.WebApi;
using Xunit;

namespace EstvTaxCalculatorFacade.Integration.Tests;

[Trait("Estv Tax Calculator", "Integration")]
public class EstvTaxCalculatorIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly HttpClient client;

    public EstvTaxCalculatorIntegrationTests(WebApplicationFactory<Startup> factory)
    {
        client = factory.CreateDefaultClient(new Uri("http://localhost/api/calculators/tax/"));
    }

    [Fact(DisplayName = "Tax Location")]
    public async Task Should_Get_TaxLocation_Successfully()
    {
        string zip = "3303";
        string city = "Zuzwil";

        var result = await client.GetFromJsonAsync<TaxLocation>($"location?zip={zip}&city={city}");

        Snapshot.Match(result);
    }
}
