using System;
using System.Threading.Tasks;
using Application.Tax.Proprietary.Abstractions;
using Application.Tax.Proprietary.Abstractions.Models.Person;
using Domain.Enums;
using FluentAssertions;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Default Basis Income Tax")]
    public class DefaultBasisIncomeTaxCalculatorTests 
        : IClassFixture<TaxCalculatorFixture<IDefaultBasisIncomeTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<IDefaultBasisIncomeTaxCalculator> _fixture;

        public DefaultBasisIncomeTaxCalculatorTests(
            TaxCalculatorFixture<IDefaultBasisIncomeTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "Default Basis Income Tax")]
        [InlineData(2018, 500_000, "Married", "ZH")]
        [InlineData(2018, 0, "Married", "ZH")]
        [InlineData(2018, 99995, "Married", "ZH")]
        [InlineData(2018, 99995, "Single", "ZH")]
        [InlineData(2019, 100000, "Single", "SG")]
        public async Task ShouldCalculateBasisIncomeTax(
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
                Name = "Burli",
                CivilStatus = status,
                TaxableAmount = income
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(
                calculationYear, canton, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result,$"Default Basis Income Tax {calculationYear}{cantonAsStr}{incomeAsDouble}{civilStatusCode}");
        }
    }
}
