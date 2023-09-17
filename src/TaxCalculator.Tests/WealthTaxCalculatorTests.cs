using System;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models.Tax;
using FluentAssertions;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Wealth")]
    public class WealthTaxCalculatorTests : IClassFixture<TaxCalculatorFixture<IWealthTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<IWealthTaxCalculator> _fixture;

        public WealthTaxCalculatorTests(TaxCalculatorFixture<IWealthTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "Wealth Tax")]
        [InlineData(2018,261,"ZH")]
        [InlineData(2019,3426,"SG")]
        public async Task ShouldCalculateWealthTax(
            int calculationYear, int municipalityId, string cantonStr)
        {
            // given
            Canton canton = Enum.Parse<Canton>(cantonStr);

            var taxPerson = new TaxPerson
            {
                Name = "Burli",
                CivilStatus = CivilStatus.Married,
                ReligiousGroupType = ReligiousGroupType.Protestant,
                TaxableIncome = 99995,
                TaxableWealth = 522000
            };

            // when
            Either<string, SingleTaxResult> result = 
                await _fixture.Calculator.CalculateAsync(
                    calculationYear, municipalityId, canton, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result,$"Theory Wealth Tax {calculationYear}{cantonStr}");
        }
    }
}
