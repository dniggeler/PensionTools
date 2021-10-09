using System;
using System.Threading.Tasks;
using FluentAssertions;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Full Tax")]
    public class FullTaxCalculatorTests : IClassFixture<TaxCalculatorFixture<IFullTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<IFullTaxCalculator> _fixture;

        public FullTaxCalculatorTests(TaxCalculatorFixture<IFullTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "Full Tax")]
        [InlineData(2018, 500_000,500_000, 4000_000,"Married", "Catholic", 261, "ZH")]
        [InlineData(2018, 0,5000,0, "Married", "Catholic", 261, "ZH")]
        [InlineData(2018, 99995, 96000, 522000, "Married", "Catholic", 261, "ZH")]
        [InlineData(2018, 99995, 99995, 522000, "Single", "Protestant", 261, "ZH")]
        [InlineData(2019, 99995, 99995, 522000, "Single", "Other", 3426, "SG")]
        [InlineData(2019, 100_000, 100_000, 500000, "Married", "Protestant", 2526, "SO")]
        public async Task ShouldCalculateOverallTax(
            int calculationYear,
            double stateIncomeAsDouble,
            double federalIncomeAsDouble,
            double wealthAsDouble,
            string civilStatusCode,
            string religiousGroupTypeCode,
            int municipalityId,
            string cantonStr)
        {
            // given
            string name = "Burli";
            Canton canton = Enum.Parse<Canton>(cantonStr);
            decimal income = Convert.ToDecimal(stateIncomeAsDouble);
            decimal federalIncome = Convert.ToDecimal(federalIncomeAsDouble);
            decimal wealth = Convert.ToDecimal(wealthAsDouble);
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);
            ReligiousGroupType religiousGroupType = Enum.Parse<ReligiousGroupType>(religiousGroupTypeCode);
            ReligiousGroupType partnerReligiousGroupType = status switch
            {
                CivilStatus.Married => religiousGroupType,
                CivilStatus.Single => ReligiousGroupType.Other,
                _ => ReligiousGroupType.Other
            };

            var taxPerson = new TaxPerson
            {
                Name = name,
                CivilStatus = status,
                ReligiousGroupType = religiousGroupType,
                PartnerReligiousGroupType = partnerReligiousGroupType,
                TaxableIncome = income,
                TaxableFederalIncome = federalIncome,
                TaxableWealth = wealth
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(
                calculationYear, municipalityId, canton, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result, $"Theory Full Tax {cantonStr}{calculationYear}{stateIncomeAsDouble}{wealthAsDouble}{civilStatusCode}");

        }
    }
}
