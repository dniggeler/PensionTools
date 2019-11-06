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
        public async Task ShouldCalculateChurchTax(
            int calculationYear, 
            double stateIncomeAsDouble, 
            double federalIncomeAsDouble, 
            double wealthAsDouble, 
            string civilStatusCode)
        {
            // given
            string name = "Burli";
            string canton = "ZH";
            decimal income = Convert.ToDecimal(stateIncomeAsDouble);
            decimal federalIncome = Convert.ToDecimal(federalIncomeAsDouble);
            decimal wealth = Convert.ToDecimal(wealthAsDouble);
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);

            var taxPerson = new ChurchTaxPerson
            {
                Canton = canton,
                Name = name,
                CivilStatus = status,
                ReligiousGroup = ReligiousGroupType.Married,
                Municipality = "Zürich",
            };

            var taxResult = new SingleTaxResult
            {
                BasisTaxAmount = new BasisTaxResult
                {
                    DeterminingFactorTaxableAmount = 99900,
                    TaxAmount = 4877M,
                },
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(calculationYear, taxPerson, taxResult);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result, $"Theory Church Tax {calculationYear}{stateIncomeAsDouble}{wealthAsDouble}{civilStatusCode}");

        }
    }
}
