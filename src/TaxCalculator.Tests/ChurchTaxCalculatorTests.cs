using System;
using System.Threading.Tasks;
using Application.Tax.Proprietary.Abstractions.Models;
using Application.Tax.Proprietary.Contracts;
using Application.Tax.Proprietary.Models;
using Domain.Enums;
using Domain.Models.Tax.Person;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Church Tax")]
    public class ChurchTaxCalculatorTests : IClassFixture<TaxCalculatorFixture<IChurchTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<IChurchTaxCalculator> _fixture;

        public ChurchTaxCalculatorTests(TaxCalculatorFixture<IChurchTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "Church Tax")]
        [InlineData(2018, 4877,252, "Married", "Protestant", 261)]
        [InlineData(2018, 4877,252, "Married", "Catholic", 261)]
        [InlineData(2018, 0,0, "Married", "Protestant", 261)]
        public async Task ShouldCalculateChurchTax(
            int calculationYear, 
            double incomeTaxAsDouble, 
            double wealthTaxAsDouble, 
            string civilStatusCode,
            string religiousGroupCode,
            int municipalityId)
        {
            // given
            string name = "Burli";
            decimal incomeTax = Convert.ToDecimal(incomeTaxAsDouble);
            decimal wealthTax = Convert.ToDecimal(wealthTaxAsDouble);
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);

            ReligiousGroupType religiousGroup =
                Enum.Parse<ReligiousGroupType>(religiousGroupCode);

            ReligiousGroupType partnerReligiousGroup 
                = ReligiousGroupType.Protestant;

            var taxPerson = new ChurchTaxPerson
            {
                Name = name,
                CivilStatus = status,
                ReligiousGroupType = religiousGroup,
                PartnerReligiousGroupType = partnerReligiousGroup,
            };

            var taxResult = new AggregatedBasisTaxResult
            {
                IncomeTax = new BasisTaxResult
                {
                    TaxAmount = incomeTax,
                },
                WealthTax = new BasisTaxResult
                {
                    TaxAmount = wealthTax,
                },
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(
                calculationYear, municipalityId, taxPerson, taxResult);

            Snapshot.Match(
                result,$"Theory Church Tax {calculationYear}{incomeTax}{wealthTax}{civilStatusCode}{religiousGroupCode}");

        }
    }
}
