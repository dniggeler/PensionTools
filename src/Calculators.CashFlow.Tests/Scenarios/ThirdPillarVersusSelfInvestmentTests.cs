using System.Threading.Tasks;
using Application.Features.TaxScenarios;
using Domain.Enums;
using Domain.Models.Scenarios;
using Domain.Models.Tax;
using Snapshooter.Xunit;
using Xunit;

namespace Calculators.CashFlow.Tests.Scenarios;

[Trait("Scenario Calculators", "3a vs Anlagekonto")]
public class ThirdPillarVersusSelfInvestmentTests : IClassFixture<CashFlowFixture<ITaxScenarioCalculator>>
{
    private readonly CashFlowFixture<ITaxScenarioCalculator> fixture;

    public ThirdPillarVersusSelfInvestmentTests(CashFlowFixture<ITaxScenarioCalculator> fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = "Excess Return on Self-Investment")]
    public async Task Calculate_Excess_Return_On_SelfInvestment()
    {
        // given
        var calculationYear = 2022;
        var finalYear = calculationYear + 20;
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
            InvestmentNetGrowthRate = 0.02M,
            InvestmentNetIncomeYield = 0.01M,
            ThirdPillarNetGrowthRate = 0.00M,
            InvestmentAmount = 7056,
        };

        // when
        var result =
            await fixture.Calculator.ThirdPillarVersusSelfInvestmentAsync(calculationYear, bfsMunicipalityId, person, scenarioModel);

        // then
        Snapshot.Match(result, opt => opt.IgnoreFields("$..Id"));
    }
}
