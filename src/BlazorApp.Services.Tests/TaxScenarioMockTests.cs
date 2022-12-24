using BlazorApp.Services.Mock;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.TaxComparison;
using Snapshooter.Xunit;

namespace BlazorApp.Services.Tests
{
    public class TaxScenarioMockTests
    {
        private ITaxScenarioService service;

        public TaxScenarioMockTests()
        {
            service = new MockTaxComparisonService();
        }

        [Fact(DisplayName = "Mock Tax Scenario Purchase")]
        public async Task Calculate_Tax_Scenario_Purchase_With_Mocked_Service()
        {
            // given
            int calculationYear = 2022;
            int yearOfCapitalBenefitWithdrawal = 2032;

            var request = new CapitalBenefitTransferInComparerRequest
            {
                Name = "Test Multi-Period Calculator",
                CalculationYear = calculationYear,
                BfsMunicipalityId = 261,
                CivilStatus = CivilStatus.Married,
                ReligiousGroup = ReligiousGroupType.Other,
                PartnerReligiousGroup = ReligiousGroupType.Other,
                TaxableIncome = 100_000,
                TaxableFederalIncome = 100_000,
                TaxableWealth = 500_000,

                WithCapitalBenefitTaxation = true,
                TransferIns = new List<SingleTransferInModel>
                {
                    new(10_000, new DateTime(calculationYear, 1, 1))
                },
                Withdrawals = new List<SingleTransferInModel>
                {
                    new(1, new DateTime(yearOfCapitalBenefitWithdrawal, 1, 1))
                },
                CapitalBenefitsBeforeWithdrawal = 400_000,
                NetWealthReturn = 0.02M
            };

            // when
            CapitalBenefitsTransferInResponse result = await service.CalculateAsync(request);

            Snapshot.Match(result);
        }
    }
}
