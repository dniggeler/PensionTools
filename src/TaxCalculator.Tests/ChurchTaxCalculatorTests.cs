using System;
using System.Threading.Tasks;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
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
        [InlineData(2018, 4877,252, "Married", "Protestant", "Zürich")]
        [InlineData(2018, 0,0, "Married", "Protestant", "Zürich")]
        public async Task ShouldCalculateChurchTax(
            int calculationYear, 
            double incomeTaxAsDouble, 
            double wealthTaxAsDouble, 
            string civilStatusCode,
            string religiousGroupCode,
            string municipality)
        {
            // given
            string name = "Burli";
            string canton = "ZH";
            decimal incomeTax = Convert.ToDecimal(incomeTaxAsDouble);
            decimal wealthTax = Convert.ToDecimal(wealthTaxAsDouble);
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);

            ReligiousGroupType religiousGroup =
                Enum.Parse<ReligiousGroupType>(religiousGroupCode);

            ReligiousGroupType partnerReligiousGroup 
                = ReligiousGroupType.Protestant;

            var taxPerson = new ChurchTaxPerson
            {
                Canton = canton,
                Name = name,
                CivilStatus = status,
                ReligiousGroup = religiousGroup,
                ReligiousGroupPartner = partnerReligiousGroup,
                Municipality = municipality,
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
            var result = await _fixture.Calculator.CalculateAsync(calculationYear, taxPerson, taxResult);

            Snapshot.Match(
                result,
                $"Theory Church Tax {calculationYear}{incomeTax}{wealthTax}{civilStatusCode}");

        }
    }
}
