using System.Threading.Tasks;
using Application.Municipality;
using Domain.Enums;
using Domain.Models.Municipality;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests;

[Trait("Connector", "Municipality")]
public class MunicipalityConnectorTests(TaxCalculatorFixture<IMunicipalityConnector> fixture)
    : IClassFixture<TaxCalculatorFixture<IMunicipalityConnector>>
{
    [Fact(DisplayName = "Get All")]
    public async Task ShouldReturnAllMunicipalities()
    {
        // given

        // when
        var result = await fixture.Service.GetAllAsync();

        Snapshot.Match(result,"Get All Municipalities");
    }

    [Fact(DisplayName = "Search")]
    public void ShouldSearchMunicipalitiesByFilter()
    {
        // given
        var filter = new MunicipalitySearchFilter
        {
            Canton = Canton.BE,
            Name = "Zuzwil",
            YearOfValidity = 2008
        };

        // when
        var result = fixture.Service.SearchAsync(filter);

        Snapshot.Match(result, $"Search Municipalities");
    }

    [Fact(DisplayName = "Get Municipality")]
    public async Task ShouldReturnMunicipalityByBfsNumber()
    {
        // given
        int bfsNumber = 261;
        int year = 2019;

        // when
        var result = await fixture.Service.GetAsync(bfsNumber, year);

        Snapshot.Match(result, $"Get Municipality {bfsNumber}");
    }

    [Fact(DisplayName = "Get All Supporting Tax Calculation")]
    public async Task ShouldReturnAllMunicipalitiesSupportingTaxCalculation()
    {
        // given

        // when
        var result =
            await fixture.Service.GetAllSupportTaxCalculationAsync();

        Snapshot.Match(result, "SupportTaxCalculation");
    }
}
