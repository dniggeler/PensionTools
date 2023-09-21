using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models.Municipality;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Connector", "Municipality")]
    public class MunicipalityConnectorTests : IClassFixture<TaxCalculatorFixture<IMunicipalityConnector>>
    {
        private readonly TaxCalculatorFixture<IMunicipalityConnector> _fixture;

        public MunicipalityConnectorTests(TaxCalculatorFixture<IMunicipalityConnector> fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Get All")]
        public async Task ShouldReturnAllMunicipalities()
        {
            // given

            // when
            var result = await _fixture.Service.GetAllAsync();

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
            var result = _fixture.Service.Search(filter);

            Snapshot.Match(result, $"Search Municipalities");
        }

        [Fact(DisplayName = "Get Municipality")]
        public async Task ShouldReturnMunicipalityByBfsNumber()
        {
            // given
            int bfsNumber = 261;
            int year = 2019;

            // when
            var result = await _fixture.Service.GetAsync(bfsNumber, year);

            Snapshot.Match(result, $"Get Municipality {bfsNumber}");
        }

        [Fact(DisplayName = "Get All Supporting Tax Calculation")]
        public async Task ShouldReturnAllMunicipalitiesSupportingTaxCalculation()
        {
            // given

            // when
            var result =
                await _fixture.Service.GetAllSupportTaxCalculationAsync();

            Snapshot.Match(result, "SupportTaxCalculation");
        }
    }
}
