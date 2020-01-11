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
    public class BasisWealthTaxCalculatorTests 
        : IClassFixture<TaxCalculatorFixture<Func<Canton, IBasisWealthTaxCalculator>>>
    {
        private readonly TaxCalculatorFixture<Func<Canton, IBasisWealthTaxCalculator>> _fixture;

        public BasisWealthTaxCalculatorTests(
            TaxCalculatorFixture<Func<Canton, IBasisWealthTaxCalculator>> fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "Basis Wealth Tax")]
        [InlineData(2018, 4_000_000, "Married", "ZH")]
        [InlineData(2018, 0, "Married", "ZH")]
        [InlineData(2018, 522000, "Married", "ZH")]
        [InlineData(2018, 522000, "Single", "ZH")]
        [InlineData(2019, 400000, "Single", "SG")]
        public async Task ShouldCalculateBasisWealthTax(
            int calculationYear, double wealthAsDouble, string civilStatusCode, string cantonStr)
        {
            // given
            Canton canton = Enum.Parse<Canton>(cantonStr);
            decimal wealth = Convert.ToDecimal(wealthAsDouble);
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);

            var taxPerson = new BasisTaxPerson
            {
                CivilStatus = status,
                TaxableAmount = wealth,
            };

            // when
            var result =
                await _fixture.Calculator(canton).CalculateAsync(
                    calculationYear, canton, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result, $"Theory Basis Wealth Tax {calculationYear}{wealthAsDouble}{civilStatusCode}");
        }
    }
}