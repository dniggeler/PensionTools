using System.Threading.Tasks;
using Domain.Enums;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
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
        [InlineData(50_000, 28680, 1_000_000, 261, 20.305)] // Zürich
        [InlineData(50_000, 28680, 1_000_000, 141, 20.214)] // Zürich
        [InlineData(50_000, 28680, 1_000_000, 3426, 20.075)] // SG
        [InlineData(50_000, 28680, 1_000_000, 2526, 20.19)] // SO
        public async Task Calculate_BreakEven_Factor(decimal retirementPension, decimal otherIncome, decimal retirementCapital, int municipalityId, decimal expectedBreakEvenFactor)
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
                TaxableFederalIncome = otherIncome,
                TaxableIncome = otherIncome,
                TaxableWealth = decimal.Zero
            };

            decimal result =
                (await fixture.Service.CalculateAsync(calculationYear, municipalityId, canton, retirementPension, retirementCapital, taxPerson))
                .IfNone(decimal.Zero);

            Assert.InRange(result, expectedBreakEvenFactor - epsilon, expectedBreakEvenFactor + epsilon);
        }
    }
}
