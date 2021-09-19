using System.Threading.Tasks;
using FluentAssertions;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
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
