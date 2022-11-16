using System;
using System.Threading.Tasks;
using FluentAssertions;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Proprietary Calculator", "Marginal Tax Curve")]
    public class MarginalTaxCurveCalculatorTests : IClassFixture<TaxCalculatorFixture<IMarginalTaxCurveCalculatorConnector>>
    {
        private readonly TaxCalculatorFixture<IMarginalTaxCurveCalculatorConnector> _fixture;

        public MarginalTaxCurveCalculatorTests(TaxCalculatorFixture<IMarginalTaxCurveCalculatorConnector> fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "Marginal Income Tax Curve")]
        [InlineData(2018, 100_000, "Married", "Catholic", 261)]
        public async Task ShouldCalculateIncomeTaxCurve(
            int calculationYear,
            double incomeAsDouble,
            string civilStatusCode,
            string religiousGroupTypeCode,
            int municipalityId)
        {
            // given
            const int lowerSalaryLimit = 0;
            const int upperSalaryLimit = 200_000;
            const int numberOfSamples = 50;

            string name = "Burli";
            decimal income = Convert.ToDecimal(incomeAsDouble);
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
            };

            // when
            var result = await _fixture.Calculator.CalculateIncomeTaxCurveAsync(
                calculationYear, municipalityId, taxPerson, lowerSalaryLimit, upperSalaryLimit, numberOfSamples);

            result.IsRight.Should().BeTrue();
            result.Match(
                Left: Assert.Fail,
                Right: r => Snapshot.Match(r, $"Income Tax Curve {calculationYear}{incomeAsDouble}{civilStatusCode}")
            );
        }

        [Theory(DisplayName = "Marginal Capital Benefit Tax Curve")]
        [InlineData(2018, 500_000, "Married", "Catholic", 261)]
        public async Task ShouldCalculateCapitalBenefitTaxCurve(
            int calculationYear,
            double incomeAsDouble,
            string civilStatusCode,
            string religiousGroupTypeCode,
            int municipalityId)
        {
            // given
            const int lowerSalaryLimit = 0;
            const int upperSalaryLimit = 2_000_000;
            const int numberOfSamples = 50;

            string name = "Burli";
            decimal income = Convert.ToDecimal(incomeAsDouble);
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);
            ReligiousGroupType religiousGroupType = Enum.Parse<ReligiousGroupType>(religiousGroupTypeCode);
            ReligiousGroupType partnerReligiousGroupType = status switch
            {
                CivilStatus.Married => religiousGroupType,
                CivilStatus.Single => ReligiousGroupType.Other,
                _ => ReligiousGroupType.Other
            };

            var taxPerson = new CapitalBenefitTaxPerson
            {
                Name = name,
                CivilStatus = status,
                ReligiousGroupType = religiousGroupType,
                PartnerReligiousGroupType = partnerReligiousGroupType,
                TaxableCapitalBenefits = income,
            };

            // when
            var result = await _fixture.Calculator.CalculateCapitalBenefitTaxCurveAsync(
                calculationYear, municipalityId, taxPerson, lowerSalaryLimit, upperSalaryLimit, numberOfSamples);

            result.IsRight.Should().BeTrue();
            result.Match(
                Left: Assert.Fail,
                Right: r => Snapshot.Match(r, $"Capital Benefit Tax Curve {calculationYear}{incomeAsDouble}{civilStatusCode}")
            );
        }
    }
}
