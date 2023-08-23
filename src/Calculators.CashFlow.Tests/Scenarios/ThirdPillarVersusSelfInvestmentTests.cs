using System.Threading.Tasks;
using Calculators.CashFlow.Models;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using Snapshooter.Xunit;
using Xunit;

namespace Calculators.CashFlow.Tests.Scenarios
{
    [Trait("Scenario Calculators", "3a vs Anlagekonto")]
    public class ThirdPillarVersusSelfInvestmentTests : IClassFixture<CashFlowFixture<ITaxScenarioCalculator>>
    {
        private readonly CashFlowFixture<ITaxScenarioCalculator> fixture;

        public ThirdPillarVersusSelfInvestmentTests(CashFlowFixture<ITaxScenarioCalculator> fixture)
        {
            this.fixture = fixture;
        }

        [Fact(DisplayName = "Zero Return on Self-Investment")]
        public async Task Calculate_Zero_Return_On_SelfInvestment()
        {
            // given
            var calculationYear = 2022;
            var finalYear = calculationYear + 10;
            var bfsMunicipalityId = 261;
            TaxPerson person = new()
            {
                Name = "Unit Test Scenario",
                CivilStatus = CivilStatus.Married,
                ReligiousGroupType = ReligiousGroupType.Other,
                PartnerReligiousGroupType = ReligiousGroupType.Other,
                TaxableIncome = 150_000,
                TaxableFederalIncome = 100_000,
                TaxableWealth = 500_000
            };

            ThirdPillarVersusSelfInvestmentScenarioModel scenarioModel = new()
            {
                FinalYear = finalYear,
                InvestmentExcessReturn = 0.03M,
                InvestmentAmount = 7056,
            };

            // when
            var result =
                await fixture.Calculator.ThirdPillarVersusSelfInvestmentAsync(calculationYear, bfsMunicipalityId, person, scenarioModel);

            // then
            Snapshot.Match(result, opt => opt.IgnoreFields("$..Id"));
        }
    }
}
