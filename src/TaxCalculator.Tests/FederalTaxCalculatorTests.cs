using System;
using System.Threading.Tasks;
using Application.Tax.Proprietary.Abstractions.Models.Person;
using Application.Tax.Proprietary.Contracts;
using Domain.Enums;
using FluentAssertions;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Federal")]
    public class FederalTaxCalculatorTests : IClassFixture<TaxCalculatorFixture<IFederalTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<IFederalTaxCalculator> _fixture;

        public FederalTaxCalculatorTests(TaxCalculatorFixture<IFederalTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "Federal Tax")]
        [InlineData(2018, 900_000, "Married")]
        [InlineData(2018, 0, "Married")]
        [InlineData(2018, 99995, "Married")]
        public async Task ShouldCalculateFederalTax(int calculationYear, double incomeAsDouble,
            string civilStatusCode)
        {
            // given
            decimal income = Convert.ToDecimal(incomeAsDouble);
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);

            var taxPerson = new FederalTaxPerson
            {
                Name = "Burli",
                CivilStatus = status,
                TaxableAmount = income
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(calculationYear, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result, $"Theory Federal Income Tax {calculationYear}{incomeAsDouble}{civilStatusCode}");

        }
    }
}
