using System.Threading.Tasks;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using PensionVersusCapitalCalculator.Abstractions;
using Xunit;

namespace PensionVersusCapitalCalculator.Tests
{
    [Trait("Tools", "Pension vs Capital")]
    public class PensionVersusCapitalCalculatorTests : IClassFixture<ToolsFixture<IPensionVersusCapitalCalculator>>
    {
        private readonly ToolsFixture<IPensionVersusCapitalCalculator> fixture;

        public PensionVersusCapitalCalculatorTests(ToolsFixture<IPensionVersusCapitalCalculator> fixture)
        {
            this.fixture = fixture;
        }

        [Theory]
        [InlineData(50_000, 1_000_000, 261, 18.838)]
        [InlineData(50_000, 1_000_000, 3426, 18.745)]
        public async Task Calculate_BreakEven_Factor(decimal retirementPension, decimal retirementCapital, int municipalityId, decimal expectedBreakEvenFactor)
        {
            const decimal epsilon = 0.001M;
            const int calculationYear = 2019;
            Canton canton = Canton.ZH;
            CivilStatus civilStatus = CivilStatus.Single;
            string name = "PensVsCap";
            ReligiousGroupType religiousGroupType = ReligiousGroupType.Protestant;

            TaxPerson taxPerson = new()
            {
                Name = name,
                CivilStatus = civilStatus,
                ReligiousGroupType = religiousGroupType,
                TaxableFederalIncome = retirementPension,
                TaxableIncome = retirementPension,
                TaxableWealth = decimal.Zero
            };

            CapitalBenefitTaxPerson capitalTaxPerson = new()
            {
                Name = name,
                CivilStatus = civilStatus,
                TaxableBenefits = retirementCapital,
                ReligiousGroupType = religiousGroupType
            };

            decimal result =
                (await fixture.Service.CalculateAsync(calculationYear, municipalityId, canton, taxPerson, capitalTaxPerson))
                .IfNone(decimal.Zero);

            Assert.InRange(result, expectedBreakEvenFactor - epsilon, expectedBreakEvenFactor + epsilon);
        }
    }
}
