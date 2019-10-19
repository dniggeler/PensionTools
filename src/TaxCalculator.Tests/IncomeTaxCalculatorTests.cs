using System.Threading.Tasks;
using FluentAssertions;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
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
            // when
            var result = await _fixture.Calculator.CalculateAsync(new TaxPerson());

            result.IsRight.Should().BeFalse();
        }
    }
}
