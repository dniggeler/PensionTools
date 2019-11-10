using System;
using System.Threading.Tasks;
using FluentAssertions;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Snapshooter.Xunit;
using Xunit;

namespace TaxCalculator.Tests
{
    [Trait("Calculator", "Full Tax")]
    public class FullTaxCalculatorTests : IClassFixture<TaxCalculatorFixture<IFullTaxCalculator>>
    {
        private readonly TaxCalculatorFixture<IFullTaxCalculator> _fixture;

        public FullTaxCalculatorTests(TaxCalculatorFixture<IFullTaxCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "Full Tax")]
        [InlineData(2018, 500_000,500_000, 4000_000,"Married")]
        [InlineData(2018, 0,5000,0, "Married")]
        [InlineData(2018, 99995, 96000, 522000, "Married")]
        public async Task ShouldCalculateOverallTax(int calculationYear, double stateIncomeAsDouble, 
            double federalIncomeAsDouble, double wealthAsDouble, string civilStatusCode)
        {
            // given
            string name = "Burli";
            string canton = "ZH";
            decimal income = Convert.ToDecimal(stateIncomeAsDouble);
            decimal federalIncome = Convert.ToDecimal(federalIncomeAsDouble);
            decimal wealth = Convert.ToDecimal(wealthAsDouble);
            CivilStatus status = Enum.Parse<CivilStatus>(civilStatusCode);

            var taxPerson = new TaxPerson
            {
                Canton = canton,
                Name = name,
                CivilStatus = status,
                ReligiousGroupType = ReligiousGroupType.Catholic,
                PartnerReligiousGroupType = ReligiousGroupType.Protestant,
                Municipality = "Zürich",
                TaxableIncome = income,
                TaxableFederalIncome = federalIncome,
                TaxableWealth = wealth
            };

            // when
            var result = await _fixture.Calculator.CalculateAsync(calculationYear, taxPerson);

            result.IsRight.Should().BeTrue();
            Snapshot.Match(result, $"Theory Full Tax {calculationYear}{stateIncomeAsDouble}{wealthAsDouble}{civilStatusCode}");

        }
    }
}
