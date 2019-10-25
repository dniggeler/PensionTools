using System.Threading.Tasks;
using FluentAssertions;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Wealth")]
    public class WealthTaxCalculatorTests : IClassFixture<TaxCalculatorFixture<IWealthTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<IWealthTaxCalculator> _fixture;

        public WealthTaxCalculatorTests(TaxCalculatorFixture<IWealthTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Wealth Tax")]
        public async Task ShouldCalculateWealthTax()
        {
            // given
            var taxPerson = new TaxPerson
            {
                Canton = "ZH",
                CalculationYear = 2018,
                CivilStatus = CivilStatus.Married,
                DenominationType = ReligiousGroupType.Married,
                Municipality = "Zürich",
                TaxableIncome = 99995,
                TaxableWealth = 522000
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(taxPerson);

            result.IsRight.Should().BeFalse();
            Snapshot.Match(result);
        }
    }
}
