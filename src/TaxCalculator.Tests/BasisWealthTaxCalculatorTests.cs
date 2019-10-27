using System.Threading.Tasks;
using FluentAssertions;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Basis Tax")]
    public class BasisWealthTaxCalculatorTests : IClassFixture<TaxCalculatorFixture<IBasisWealthTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<IBasisWealthTaxCalculator> _fixture;

        public BasisWealthTaxCalculatorTests(TaxCalculatorFixture<IBasisWealthTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Wealth Tax")]
        public async Task ShouldCalculateBasisIncomeTax()
        {
            // given
            int calculationYear = 2018;

            var taxPerson = new BasisTaxPerson
            {
                Canton = "ZH",
                CivilStatus = CivilStatus.Married,
                TaxableAmount = 522000
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(calculationYear, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result);
        }
    }
}