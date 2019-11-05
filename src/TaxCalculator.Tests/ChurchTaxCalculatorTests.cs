using System;
using System.Threading.Tasks;
using FluentAssertions;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Church Tax")]
    public class ChurchTaxCalculatorTests : IClassFixture<TaxCalculatorFixture<IChurchTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<IChurchTaxCalculator> _fixture;

        public ChurchTaxCalculatorTests(TaxCalculatorFixture<IChurchTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "Church Tax")]
        [InlineData(2018, 500_000,500_000, 4000_000,"Married")]
        [InlineData(2018, 0,5000,0, "Married")]
        [InlineData(2018, 99995, 96000, 522000, "Married")]
        public async Task ShouldCalculateChurchTax(int calculationYear, double stateIncomeAsDouble, 
            double federalIncomeAsDouble, double wealthAsDouble, string civilStatusCode)
        {
            // given
            string name = "Burli";
            string canton = "ZH";
            decimal income = Convert.ToDecimal(stateIncomeAsDouble);
            decimal federalIncome = Convert.ToDecimal(federalIncomeAsDouble);
            decimal wealth = Convert.ToDecimal(wealthAsDouble);
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);

            var taxPerson = new TaxPerson()
            {
                Canton = canton,
                Name = name,
                CivilStatus = status,
                DenominationType = ReligiousGroupType.Married,
                Municipality = "Zürich",
                TaxableIncome = income,
                TaxableFederalIncome = federalIncome,
                TaxableWealth = wealth
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(calculationYear, taxPerson,null);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result, $"Theory Church Tax {calculationYear}{stateIncomeAsDouble}{wealthAsDouble}{civilStatusCode}");

        }
    }
}
