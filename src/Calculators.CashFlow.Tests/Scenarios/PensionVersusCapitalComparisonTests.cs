using System.Threading.Tasks;
using Application.Features.TaxScenarios;
using Domain.Enums;
using Domain.Models.Tax;
using Snapshooter.Xunit;
using Xunit;

namespace Calculators.CashFlow.Tests.Scenarios;

[Trait("Scenario Calculators", "Pension vs Capital")]
public class PensionVersusCapitalComparisonTests : IClassFixture<CashFlowFixture<ITaxScenarioCalculator>>
{
    private readonly CashFlowFixture<ITaxScenarioCalculator> fixture;

    public PensionVersusCapitalComparisonTests(CashFlowFixture<ITaxScenarioCalculator> fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = "City of Zurich")]
    public async Task Calculate_Comparison_For_Zurich()
    {
        // given
        var calculationYear = 2019;
        var bfsMunicipalityId = 261;
        decimal retirementPension = 50_000;
        decimal retirementCapital = 500_000;
        decimal yearConsumptionAmount = 78_800;
        decimal netWealthReturn = 0.03m;

        TaxPerson person = new()
        {
            Name = "Unit Test Purchase",
            CivilStatus = CivilStatus.Single,
            ReligiousGroupType = ReligiousGroupType.Other,
            PartnerReligiousGroupType = ReligiousGroupType.Other,
            TaxableIncome = 28_800,
            TaxableFederalIncome = 28_800,
            TaxableWealth = 500000
        };

        // when
        var result = await fixture.Calculator.PensionVersusCapitalComparisonAsync(
            calculationYear, bfsMunicipalityId, yearConsumptionAmount, retirementPension, retirementCapital, netWealthReturn, person);

        // then
        Snapshot.Match(result, opt => opt.IgnoreFields("$..Id"));
    }
}
