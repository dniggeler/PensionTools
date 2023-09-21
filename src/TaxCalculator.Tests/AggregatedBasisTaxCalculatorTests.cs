using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models.Tax;
using FluentAssertions;
using PensionCoach.Tools.TaxCalculator.Abstractions;
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
            Canton canton = Canton.ZH;

            var taxPerson = new TaxPerson
            {
                Name = "Burli",
                CivilStatus = CivilStatus.Married,
                ReligiousGroupType = ReligiousGroupType.Protestant,
                TaxableIncome = 99995,
                TaxableWealth = 522000
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(
                calculationYear, canton, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result);
        }
    }
}
