using System;
using System.Threading.Tasks;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Capital Benefit Tax")]
    public class CapitalBenefitTaxCalculatorTests 
        : IClassFixture<TaxCalculatorFixture<Func<Canton, ICapitalBenefitTaxCalculator>>>
    {
        private readonly TaxCalculatorFixture<Func<Canton, ICapitalBenefitTaxCalculator>> _fixture;

        public CapitalBenefitTaxCalculatorTests(
            TaxCalculatorFixture<Func<Canton, ICapitalBenefitTaxCalculator>> fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "Capital Benefit Tax")]
        [InlineData(2018, 2_000_000, "Single", "Protestant", 261, "ZH")]
        [InlineData(2018, 1_000_000, "Single", "Protestant", 261, "ZH")]
        [InlineData(2018, 2_000_000, "Married", "Catholic", 261, "ZH")]
        [InlineData(2018, 0, "Married", "Protestant", 261, "ZH")]
        [InlineData(2019, 2_000_000, "Single", "Protestant", 2526, "SO")]
        public async Task ShouldCalculateCapitalBenefitTax(
            int calculationYear, 
            double capitalBenefitAsDouble, 
            string civilStatusCode,
            string religiousGroupCode,
            int municipalityId,
            string cantonStr)
        {
            // given
            string name = "Burli";
            Canton canton = Enum.Parse<Canton>(cantonStr);
            decimal capitalBenefitAmount = Convert.ToDecimal(capitalBenefitAsDouble);
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);

            ReligiousGroupType religiousGroup =
                Enum.Parse<ReligiousGroupType>(religiousGroupCode);

            ReligiousGroupType partnerReligiousGroup 
                = ReligiousGroupType.Protestant;

            var taxPerson = new CapitalBenefitTaxPerson
            {
                Name = name,
                CivilStatus = status,
                ReligiousGroupType = religiousGroup,
                PartnerReligiousGroupType = partnerReligiousGroup,
                TaxableBenefits = capitalBenefitAmount,
            };

            // when
            var result =
                await _fixture.Calculator(canton).CalculateAsync(
                    calculationYear, municipalityId, canton, taxPerson);

            Snapshot.Match(
                result,
                $"Theory Capital Benefit Tax {calculationYear}{capitalBenefitAmount}{civilStatusCode}{religiousGroupCode}");
        }
    }
}
