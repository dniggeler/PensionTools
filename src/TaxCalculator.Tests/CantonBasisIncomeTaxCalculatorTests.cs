using System;
using System.Threading.Tasks;
using Domain.Enums;
using FluentAssertions;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Canton Basis Income Tax Calculators")]
    public class CantonBasisIncomeTaxCalculatorTests
        : IClassFixture<TaxCalculatorFixture<Func<Canton, IBasisIncomeTaxCalculator>>>
    {
        private readonly TaxCalculatorFixture<Func<Canton, IBasisIncomeTaxCalculator>> _fixture;

        public CantonBasisIncomeTaxCalculatorTests(
            TaxCalculatorFixture<Func<Canton, IBasisIncomeTaxCalculator>> fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "Canton Basis Income Tax")]
        [InlineData(2019, 100000, "Single", "SG")]
        [InlineData(2019, 100000, "Married", "SG")]
        public async Task ShouldCalculateBasisIncomeTaxWithCantonCalculator(
            int calculationYear,
            double incomeAsDouble,
            string civilStatusCode,
            string cantonAsStr)
        {
            // given
            Canton canton = Enum.Parse<Canton>(cantonAsStr);
            decimal income = Convert.ToDecimal(incomeAsDouble);
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);

            var taxPerson = new BasisTaxPerson
            {
                Name = "Simon",
                CivilStatus = status,
                TaxableAmount = income
            };

            // when
            var result =
                await _fixture.Calculator(canton).CalculateAsync(
                    calculationYear, canton, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result,
                $"Canton Basis Income Tax {calculationYear}{cantonAsStr}{incomeAsDouble}{civilStatusCode}");
        }

        [Fact(DisplayName = "Null Tax calculator for missing canton")]
        public async Task ShouldReturnMissingBasisIncomeTaxCalculator()
        {
            // given
            int calculationYear = 2019;
            Canton canton = Canton.VS;
            decimal income = 0;
            CivilStatus status = CivilStatus.Married;

            var taxPerson = new BasisTaxPerson
            {
                CivilStatus = status,
                TaxableAmount = income
            };

            // when
            var result =
                await _fixture.Calculator(canton).CalculateAsync(
                    calculationYear, canton, taxPerson);

            result.IsLeft.Should().BeTrue();
        }
    }
}
