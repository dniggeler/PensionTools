using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Tax.Estv.Client;
using Application.Tax.Estv.Client.Models;
using Infrastructure.EstvTaxCalculator;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Xunit;
using Xunit;

namespace EstvTaxCalculatorFacade.Integration.Tests;

public class EstvMunicipalitySearchIntegrationTests
{
    const string EstvTaxCalculatorBaseUrl = "https://swisstaxcalculator.estv.admin.ch/delegate/ost-integration/v1/lg-proxy/operation/c3b67379_ESTV/";
    private readonly ServiceProvider provider;

    public EstvMunicipalitySearchIntegrationTests()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddEstvTaxCalculatorClient(EstvTaxCalculatorBaseUrl, false); // Disable cache
        provider = services.BuildServiceProvider();
    }

    [Fact(DisplayName = "Search by municipality name")]
    public async Task Should_Search_Municipality_By_Name_Successfully()
    {
        // given
        string zip = "";
        string city = "Zürich";
        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();

        // when
        TaxLocation[] result = await estvClient.GetTaxLocationsAsync(zip, city);

        // then
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        // Note: API may return "Zurich" or "Zürich" depending on language settings
        Assert.All(result, m =>
        {
            var cityOrName = m.City ?? m.BfsName;
            Assert.True(cityOrName.Contains("Zürich", StringComparison.OrdinalIgnoreCase) ||
                        cityOrName.Contains("Zurich", StringComparison.OrdinalIgnoreCase),
                $"Expected 'Zürich' or 'Zurich' in '{cityOrName}'");
        });
    }

    [Fact(DisplayName = "Search by partial municipality name")]
    public async Task Should_Search_Municipality_By_Partial_Name_Successfully()
    {
        // given
        string zip = "3303";
        string city = "Zuzwil";
        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();

        // when
        TaxLocation[] result = await estvClient.GetTaxLocationsAsync(zip, city);

        // then
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Snapshot.Match(result, "Search by zip and city Zuzwil");
    }

    [Theory(DisplayName = "Search municipalities by ZIP code")]
    [InlineData("8001", "Zürich")]
    [InlineData("3011", "Bern")]
    [InlineData("1201", "Genève")]
    [InlineData("1003", "Lausanne")]
    public async Task Should_Search_Municipality_By_Zip_Code_Successfully(string zip, string expectedCity)
    {
        // given
        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();

        // when
        var result = await estvClient.GetTaxLocationsAsync(zip, "");

        // then
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var municipality = result.FirstOrDefault();
        Assert.NotNull(municipality);
        Assert.Contains(expectedCity, municipality.City ?? municipality.BfsName, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Search with partial filter returns results")]
    public async Task Should_Return_Results_With_Partial_Filter()
    {
        // given
        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();

        // when - Search for municipalities starting with "A"
        var result = await estvClient.GetTaxLocationsAsync("", "Aar");

        // then
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact(DisplayName = "Search non-existent municipality")]
    public async Task Should_Return_Empty_For_Non_Existent_Municipality()
    {
        // given
        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();

        // when
        var result = await estvClient.GetTaxLocationsAsync("", "XYZ123NonExistent");

        // then
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact(DisplayName = "Search validates municipality has tax location ID")]
    public async Task Should_Verify_Municipality_Has_Tax_Location_Id()
    {
        // given
        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();

        // when
        var result = await estvClient.GetTaxLocationsAsync("8001", "Zürich");

        // then
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var zurich = result.FirstOrDefault();
        Assert.NotNull(zurich);
        Assert.True(zurich.Id > 0, "Municipality should have a valid tax location ID");
        Assert.Equal(261, zurich.BfsId); // Zürich BFS number
    }

    [Fact(DisplayName = "Search by ZIP code finds municipality")]
    public async Task Should_Find_Municipality_By_Zip()
    {
        // given
        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();
        string zipCode = "8001"; // Zürich

        // when
        var result = await estvClient.GetTaxLocationsAsync(zipCode, "");

        // then
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var municipality = result.FirstOrDefault();
        Assert.NotNull(municipality);
        Assert.Equal(261, municipality.BfsId); // Zürich BFS number
    }

    [Theory(DisplayName = "Search different cantons")]
    [InlineData("3000", "BE")] // Bern, Canton BE
    [InlineData("8000", "ZH")] // Zürich, Canton ZH
    [InlineData("1200", "GE")] // Geneva, Canton GE
    [InlineData("4000", "BS")] // Basel, Canton BS
    [InlineData("6900", "TI")] // Lugano, Canton TI
    public async Task Should_Search_Municipalities_From_Different_Cantons(string zip, string expectedCanton)
    {
        // given
        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();

        // when
        var result = await estvClient.GetTaxLocationsAsync(zip, "");

        // then
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var municipality = result.FirstOrDefault();
        Assert.NotNull(municipality);
        Assert.Equal(expectedCanton, municipality.Canton);
    }

    [Fact(DisplayName = "Search validates result structure")]
    public async Task Should_Return_Complete_Municipality_Data()
    {
        // given
        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();

        // when
        var result = await estvClient.GetTaxLocationsAsync("8001", "Zürich");

        // then
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var municipality = result.First();
            
        Assert.True(municipality.Id > 0, "Should have tax location ID");
        Assert.True(municipality.BfsId > 0, "Should have BFS ID");
        Assert.True(municipality.CantonId > 0, "Should have canton ID");
        Assert.NotNull(municipality.ZipCode);
        Assert.NotNull(municipality.Canton);
        Assert.NotNull(municipality.BfsName);
    }

    [Fact(DisplayName = "Search handles multiple results for same city name")]
    public async Task Should_Return_Multiple_Results_For_Common_Name()
    {
        // given
        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();

        // when  
        // "Neuheim" exists in multiple cantons
        var result = await estvClient.GetTaxLocationsAsync("", "Ber");

        // then
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        // Should have multiple municipalities matching "Ber" (Bern, Bernex, etc.)
        Assert.True(result.Length > 1, "Should find multiple municipalities matching 'Ber'");
    }

    [Fact(DisplayName = "Snapshot test for complete search result")]
    public async Task Should_Match_Complete_Search_Result_Snapshot()
    {
        // given
        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();

        // when
        var result = await estvClient.GetTaxLocationsAsync("3303", "Zuzwil");

        // then
        Snapshot.Match(result, "Complete Zuzwil search result");
    }

    [Theory(DisplayName = "Search validates ZIP format handling")]
    [InlineData("8001")]
    [InlineData("3011")]
    [InlineData("1003")]
    public async Task Should_Handle_Different_Zip_Formats(string zip)
    {
        // given
        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();

        // when
        var result = await estvClient.GetTaxLocationsAsync(zip, "");

        // then
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.All(result, m => Assert.Equal(zip, m.ZipCode));
    }
}
