using System.Threading.Tasks;
using FluentAssertions;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
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
            var taxPerson = new TaxPerson
            {
                Canton = "ZH",
                CalculationYear = 2018,
                CivilStatus = CivilStatus.Married,
                DenominationType = DenominationType.Married,
                Municipality = "Zürich",
                TaxableIncome = 99995
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(taxPerson);

            result.IsRight.Should().BeFalse();
            Snapshot.Match(result);
        }
    }
}
