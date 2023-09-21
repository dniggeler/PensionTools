using System;
using System.Threading.Tasks;
using Application.Tax.Proprietary;
using Application.Tax.Proprietary.Abstractions;
using Application.Tax.Proprietary.Abstractions.Models.Person;
using Domain.Enums;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Federal Capital Benefit Tax")]
    public class FederalCapitalBenefitTaxCalculatorTests 
        : IClassFixture<TaxCalculatorFixture<IFederalCapitalBenefitTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<IFederalCapitalBenefitTaxCalculator> _fixture;

        public FederalCapitalBenefitTaxCalculatorTests(
            TaxCalculatorFixture<IFederalCapitalBenefitTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "Federal Capital Benefit Tax")]
        [InlineData(2018, 2_000_000, "Single")]
        [InlineData(2018, 1_000_000, "Single")]
        [InlineData(2018, 2_000_000, "Married")]
        [InlineData(2018, 0, "Married")]
        public async Task ShouldCalculateCapitalBenefitTax(
            int calculationYear, 
            double capitalBenefitAsDouble, 
            string civilStatusCode)
        {
            // given
            string name = "Burli";
            decimal capitalBenefitAmount = Convert.ToDecimal(capitalBenefitAsDouble);
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);

            var taxPerson = new FederalTaxPerson
            {
                Name = name,
                CivilStatus = status,
                TaxableAmount = capitalBenefitAmount,
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(calculationYear, taxPerson);

            Snapshot.Match(
                result,
                $"Theory Federal Capital Benefit Tax {calculationYear}{capitalBenefitAmount}{civilStatusCode}");
        }
    }
}
