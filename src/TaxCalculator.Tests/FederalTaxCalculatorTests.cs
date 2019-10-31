using System.Threading.Tasks;
using FluentAssertions;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Federal")]
    public class FederalTaxCalculatorTests : IClassFixture<TaxCalculatorFixture<IFederalTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<IFederalTaxCalculator> _fixture;

        public FederalTaxCalculatorTests(TaxCalculatorFixture<IFederalTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Federal Tax")]
        public async Task ShouldCalculateIncomeTax()
        {
            // given
            var calculationYear = 2018;
            var taxPerson = new BasisTaxPerson
            {
                Canton = "ZH",
                CivilStatus = CivilStatus.Married,
                TaxableAmount = 99995
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(calculationYear, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result);
        }
    }
}
