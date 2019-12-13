using System;
using System.Threading.Tasks;
using FluentAssertions;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
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

            Snapshot.Match(result,$"Get All Municipalities");
        }
    }
}