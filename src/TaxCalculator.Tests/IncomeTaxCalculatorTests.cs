using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models.Tax;
using FluentAssertions;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Income")]
    public class IncomeTaxCalculatorTests : IClassFixture<TaxCalculatorFixture<IIncomeTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<IIncomeTaxCalculator> _fixture;

        public IncomeTaxCalculatorTests(TaxCalculatorFixture<IIncomeTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Income Tax")]
        public async Task ShouldCalculateIncomeTax()
        {
            // given
            var calculationYear = 2018;
            var municipalityId = 261;
            Canton canton = Canton.ZH;
            var taxPerson = new TaxPerson
            {
                Name = "Burli",
                CivilStatus = CivilStatus.Married,
                ReligiousGroupType = ReligiousGroupType.Protestant,
                TaxableIncome = 99995
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(
                calculationYear, municipalityId, canton, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result);
        }
    }
}
