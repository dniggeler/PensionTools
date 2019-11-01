using System;
using System.Threading.Tasks;
using FluentAssertions;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Basis Tax")]
    public class BasisIncomeTaxCalculatorTests : IClassFixture<TaxCalculatorFixture<IBasisIncomeTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<IBasisIncomeTaxCalculator> _fixture;

        public BasisIncomeTaxCalculatorTests(TaxCalculatorFixture<IBasisIncomeTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "Basis Income Tax")]
        [InlineData(2018, 500_000, "Married")]
        [InlineData(2018, 0, "Married")]
        [InlineData(2018, 99995, "Married")]
        [InlineData(2018, 99995, "Single")]
        public async Task ShouldCalculateBasisIncomeTax(int calculationYear, double wealthAsDouble,
            string civilStatusCode)
        {
            // given
            string canton = "ZH";
            decimal wealth = Convert.ToDecimal(wealthAsDouble);
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);

            var taxPerson = new BasisTaxPerson
            {
                Canton = canton,
                CivilStatus = status,
                TaxableAmount = wealth
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(calculationYear, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result,$"Theory Basis Income Tax {calculationYear}{wealthAsDouble}{civilStatusCode}");
        }
    }
}