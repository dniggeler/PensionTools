using System.Threading.Tasks;
using FluentAssertions;
using LanguageExt;
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
            var calculationYear = 2018;
            var taxPerson = new TaxPerson
            {
                Canton = "ZH",
                CivilStatus = CivilStatus.Married,
                ReligiousGroupType = ReligiousGroupType.Protestant,
                Municipality = "Zürich",
                TaxableIncome = 99995,
                TaxableWealth = 522000
            };

            // when
            Either<string, SingleTaxResult> result = 
                await _fixture.Calculator.CalculateAsync(calculationYear, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result);
        }
    }
}
