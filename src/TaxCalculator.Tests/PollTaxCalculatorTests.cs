using System;
using System.Threading.Tasks;
using Application.Tax.Proprietary;
using Application.Tax.Proprietary.Abstractions;
using Application.Tax.Proprietary.Abstractions.Models.Person;
using Domain.Enums;
using FluentAssertions;
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
        [InlineData(2018, "Married", "ZH", 261)]
        [InlineData(2018, "Married", "BE", 557)]
        [InlineData(2018, "Single", "ZH", 261)]
        [InlineData(2018, "Single", "BE", 557)]
        [InlineData(2019, "Single", "SO", 2526)]
        [InlineData(2019, "Married", "SO", 2526)]
        public async Task ShouldCalculatePollTax(
            int calculationYear, string civilStatusCode, string cantonAsStr, int municipalityId)
        {
            // given
            string name = "Burli";
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);
            Canton canton = Enum.Parse<Canton>(cantonAsStr);

            var taxPerson = new PollTaxPerson
            {
                Name = name,
                CivilStatus = status,
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(
                calculationYear, municipalityId, canton, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result,$"Theory Poll Tax {calculationYear}{civilStatusCode}{canton}");
        }
    }
}
