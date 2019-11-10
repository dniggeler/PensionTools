using System;
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

        [Theory(DisplayName = "State Tax")]
        [InlineData(2018, 500_000, 4000_000,"Married")]
        [InlineData(2018, 0,0, "Married")]
        [InlineData(2018, 99995, 522000, "Married")]
        public async Task ShouldCalculateStateTax(int calculationYear, double incomeAsDouble, double wealthAsDouble,
            string civilStatusCode)
        {
            // given
            string name = "Burli";
            string canton = "ZH";
            decimal income = Convert.ToDecimal(incomeAsDouble);
            decimal wealth = Convert.ToDecimal(wealthAsDouble);
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);

            var taxPerson = new TaxPerson
            {
                Canton = canton,
                Name = name,
                CivilStatus = status,
                ReligiousGroupType = ReligiousGroupType.Protestant,
                Municipality = "Zürich",
                TaxableIncome = income,
                TaxableWealth = wealth
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(calculationYear, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result, $"Theory State Tax {calculationYear}{incomeAsDouble}{wealthAsDouble}{civilStatusCode}");

        }
    }
}
