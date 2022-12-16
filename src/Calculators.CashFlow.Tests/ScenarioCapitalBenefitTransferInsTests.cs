using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxComparison;
using Snapshooter.Xunit;
using Xunit;

namespace Calculators.CashFlow.Tests
{
    [Trait("Higher Level Calculators", "Transfer-Ins")]
    public class ScenarioCapitalBenefitTransferInsTests : IClassFixture<CashFlowFixture<ITaxScenarioCalculator>>
    {
        private readonly CashFlowFixture<ITaxScenarioCalculator> fixture;

        public ScenarioCapitalBenefitTransferInsTests(CashFlowFixture<ITaxScenarioCalculator> fixture)
        {
            this.fixture = fixture;
        }

        [Fact(DisplayName = "Single Transfer-In With Withdrawal in 10 Years")]
        public async Task Calculate_Single_TransferIn_With_Withdrawal_In_10_Years()
        {
            // given
            int calculationYear = 2022;
            int bfsMunicipalityId = 261;
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

            TransferInCapitalBenefitsScenarioModel scenarioModel = new()
            {
                NetReturnRate = 0.0M,
                TransferIns = new List<SingleTransferInModel>
                {
                    new(10000, new DateTime(2022, 1, 1)),
                },
                WithCapitalBenefitTaxation = true,
                YearOfCapitalBenefitWithdrawal = 2032,
                FinalRetirementCapital = 800_000
            };

            // when
            var result = await fixture.Calculator.TransferInCapitalBenefitsAsync(
                calculationYear, bfsMunicipalityId, person, scenarioModel);

            // then
            Snapshot.Match(result, opt => opt.IgnoreFields("$..Id"));
        }
    }
}
