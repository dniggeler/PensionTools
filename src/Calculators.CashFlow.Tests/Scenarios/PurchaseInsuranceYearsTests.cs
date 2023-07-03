using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Calculators.CashFlow.Models;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxComparison;
using Snapshooter.Xunit;
using Xunit;

namespace Calculators.CashFlow.Tests.Scenarios
{
    [Trait("Scenario Calculators", "Purchase Insurance Years")]
    public class PurchaseInsuranceYearsTests : IClassFixture<CashFlowFixture<ITaxScenarioCalculator>>
    {
        private readonly CashFlowFixture<ITaxScenarioCalculator> fixture;

        public PurchaseInsuranceYearsTests(CashFlowFixture<ITaxScenarioCalculator> fixture)
        {
            this.fixture = fixture;
        }

        [Fact(DisplayName = "Single Purchase With Withdrawal in 10 Years")]
        public async Task Calculate_Single_Purchase_With_Withdrawal_In_10_Years()
        {
            // given
            var calculationYear = 2022;
            var bfsMunicipalityId = 261;
            TaxPerson person = new()
            {
                Name = "Unit Test Purchase",
                CivilStatus = CivilStatus.Married,
                ReligiousGroupType = ReligiousGroupType.Other,
                PartnerReligiousGroupType = ReligiousGroupType.Other,
                TaxableIncome = 150_000,
                TaxableFederalIncome = 100_000,
                TaxableWealth = 500_000
            };

            CapitalBenefitTransferInsScenarioModel scenarioModel = new()
            {
                NetReturnCapitalBenefits = 0.0M,
                TransferIns = new List<SingleTransferInModel>
                {
                    new(10000, new DateTime(2022, 1, 1)),
                },
                WithCapitalBenefitWithdrawal = true,
                CapitalBenefitsBeforeWithdrawal = 800_000,
                Withdrawals = new List<SingleTransferInModel>
                {
                    new(0.5M, new DateTime(2032, 12, 31)),
                    new(1M, new DateTime(2033, 12, 31)),
                },
            };

            // when
            var result = await fixture.Calculator.CapitalBenefitTransferInsAsync(
                calculationYear, bfsMunicipalityId, person, scenarioModel);

            // then
            Snapshot.Match(result, opt => opt.IgnoreFields("$..Id"));
        }
    }
}
