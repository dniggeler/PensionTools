using System.Threading.Tasks;
using FluentAssertions;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
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

        [Fact(DisplayName = "State Tax")]
        public async Task ShouldCalculateStateTax()
        {
            // given
            int calculationYear = 2018;

            var taxPerson = new TaxPerson
            {
                Canton = "ZH",
                Name = "Burli",
                CivilStatus = CivilStatus.Married,
                DenominationType = ReligiousGroupType.Married,
                Municipality = "Zürich",
                TaxableIncome = 99995,
                TaxableWealth = 522000
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(calculationYear, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result);
        }
    }
}
