using System;
using System.Threading.Tasks;
using Domain.Enums;
using FluentAssertions;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "State Tax")]
    public class StateTaxCalculatorTests : IClassFixture<TaxCalculatorFixture<IStateTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<IStateTaxCalculator> _fixture;

        public StateTaxCalculatorTests(TaxCalculatorFixture<IStateTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "State Tax")]
        [InlineData(2018, 500_000, 4000_000, "Married", "Protestant")]
        [InlineData(2018, 0,0, "Married", "Protestant")]
        [InlineData(2018, 99995, 522000, "Married", "Protestant")]
        public async Task ShouldCalculateStateTax(
            int calculationYear,
            double incomeAsDouble,
            double wealthAsDouble,
            string civilStatusCode,
            string religiousGroupTypeCode)
        {
            // given
            string name = "Burli";
            int municipalityId = 261;
            Canton canton = Canton.ZH;
            decimal income = Convert.ToDecimal(incomeAsDouble);
            decimal wealth = Convert.ToDecimal(wealthAsDouble);
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);
            ReligiousGroupType religiousGroupType = Enum.Parse<ReligiousGroupType>(religiousGroupTypeCode);
            ReligiousGroupType partnerReligiousGroupType = ReligiousGroupType.Protestant;

            var taxPerson = new TaxPerson
            {
                Name = name,
                CivilStatus = status,
                ReligiousGroupType = religiousGroupType,
                PartnerReligiousGroupType = partnerReligiousGroupType,
                TaxableIncome = income,
                TaxableWealth = wealth
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(
                calculationYear, municipalityId, canton, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result, $"Theory State Tax {calculationYear}{incomeAsDouble}{wealthAsDouble}{civilStatusCode}");

        }
    }
}
