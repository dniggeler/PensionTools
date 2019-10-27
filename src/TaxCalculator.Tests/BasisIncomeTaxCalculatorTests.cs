using System.Threading.Tasks;
using FluentAssertions;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Income Tax")]
    public class BasisIncomeTaxCalculatorTests : IClassFixture<TaxCalculatorFixture<IBasisIncomeTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<IBasisIncomeTaxCalculator> _fixture;

        public BasisIncomeTaxCalculatorTests(TaxCalculatorFixture<IBasisIncomeTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Basis Income Tax")]
        public async Task ShouldCalculateBasisIncomeTax()
        {
            // given
            int calculationYear = 2018;

            var taxPerson = new BasisTaxPerson
            {
                Canton = "ZH",
                CivilStatus = CivilStatus.Married,
                TaxableAmount = 99995
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(calculationYear, taxPerson);

            result.IsRight.Should().BeFalse();
            Snapshot.Match(result);
        }
    }
}
