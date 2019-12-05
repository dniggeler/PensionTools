using System;
using System.Threading.Tasks;
using FluentAssertions;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Poll Tax")]
    public class PollTaxCalculatorTests : IClassFixture<TaxCalculatorFixture<IPollTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<IPollTaxCalculator> _fixture;

        public PollTaxCalculatorTests(TaxCalculatorFixture<IPollTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "Poll Tax")]
        [InlineData(2018, "Married", "ZH")]
        [InlineData(2018, "Married", "BE")]
        [InlineData(2018, "Single", "ZH")]
        [InlineData(2018, "Single", "BE")]
        public async Task ShouldCalculatePollTax(int calculationYear, string civilStatusCode, 
            string canton)
        {
            // given
            string name = "Burli";
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);

            var taxPerson = new PollTaxPerson
            {
                Name = name,
                Canton = Enum.Parse<Canton>(canton),
                CivilStatus = status,
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(calculationYear, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result,$"Theory Poll Tax {calculationYear}{civilStatusCode}{canton}");
        }
    }
}