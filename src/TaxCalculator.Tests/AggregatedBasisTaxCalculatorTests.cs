using System.Threading.Tasks;
using FluentAssertions;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Aggregated Tax")]
    public class AggregatedBasisTaxCalculatorTests : IClassFixture<TaxCalculatorFixture<IAggregatedBasisTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<IAggregatedBasisTaxCalculator> _fixture;

        public AggregatedBasisTaxCalculatorTests(TaxCalculatorFixture<IAggregatedBasisTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Below Max but Above Min Level")]
        public async Task ShouldCalculateIncomeTaxWhenBelowMaxButAboveMinLevel()
        {
            // given
            int calculationYear = 2018;

            var taxPerson = new TaxPerson
            {
                Canton = "ZH",
                Name = "Burli",
                CivilStatus = CivilStatus.Married,
                DenominationType = ReligiousGroupType.Married,
                Municipality = "Zürich",
                TaxableIncome = 99995,
                TaxableWealth = 522000
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(calculationYear, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result);
        }
    }
}
