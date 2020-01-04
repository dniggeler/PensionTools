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
    [Trait("Calculator", "Basis Tax")]
    public class BasisWealthTaxCalculatorTests : IClassFixture<TaxCalculatorFixture<IBasisWealthTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<IBasisWealthTaxCalculator> _fixture;

        public BasisWealthTaxCalculatorTests(TaxCalculatorFixture<IBasisWealthTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "Basis Wealth Tax")]
        [InlineData(2018, 4_000_000, "Married")]
        [InlineData(2018, 0, "Married")]
        [InlineData(2018, 522000, "Married")]
        [InlineData(2018, 522000, "Single")]
        public async Task ShouldCalculateBasisWealthTax(int calculationYear, double wealthAsDouble,
            string civilStatusCode)
        {
            // given
            Canton canton = Canton.ZH;
            decimal wealth = Convert.ToDecimal(wealthAsDouble);
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);

            var taxPerson = new BasisTaxPerson
            {
                CivilStatus = status,
                TaxableAmount = wealth,
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(
                calculationYear, canton, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result, $"Theory Basis Wealth Tax {calculationYear}{wealthAsDouble}{civilStatusCode}");
        }
    }
}