using System;
using System.Threading.Tasks;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Capital Benefit Tax")]
    public class CapitalBenefitTaxCalculatorTests 
        : IClassFixture<TaxCalculatorFixture<ICapitalBenefitTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<ICapitalBenefitTaxCalculator> _fixture;

        public CapitalBenefitTaxCalculatorTests(TaxCalculatorFixture<ICapitalBenefitTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "Capital Benefit Tax")]
        [InlineData(2018, 2_000_000, "Single", "Protestant", "Zürich")]
        [InlineData(2018, 1_000_000, "Single", "Protestant", "Zürich")]
        [InlineData(2018, 2_000_000, "Married", "Catholic", "Zürich")]
        [InlineData(2018, 0, "Married", "Protestant", "Zürich")]
        public async Task ShouldCalculateCapitalBenefitTax(
            int calculationYear, 
            double capitalBenefitAsDouble, 
            string civilStatusCode,
            string religiousGroupCode,
            string municipality)
        {
            // given
            string name = "Burli";
            Canton canton = Canton.ZH;
            decimal capitalBenefitAmount = Convert.ToDecimal(capitalBenefitAsDouble);
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);

            ReligiousGroupType religiousGroup =
                Enum.Parse<ReligiousGroupType>(religiousGroupCode);

            ReligiousGroupType partnerReligiousGroup 
                = ReligiousGroupType.Protestant;

            var taxPerson = new CapitalBenefitTaxPerson
            {
                Canton = canton,
                Name = name,
                CivilStatus = status,
                Municipality = municipality,
                ReligiousGroupType = religiousGroup,
                PartnerReligiousGroupType = partnerReligiousGroup,
                TaxableBenefits = capitalBenefitAmount,
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(calculationYear, taxPerson);

            Snapshot.Match(
                result,
                $"Theory Capital Benefit Tax {calculationYear}{capitalBenefitAmount}{civilStatusCode}{religiousGroupCode}");
        }
    }
}
