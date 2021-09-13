using System.Collections.Generic;
using System.Threading.Tasks;
using Calculators.CashFlow.Models;
using PensionCoach.Tools.CommonTypes;
using Snapshooter.Xunit;
using Xunit;

namespace Calculators.CashFlow.Tests
{
    [Trait("Higher Level Calculators", "Multi-Period Calculator")]
    public class MultiPeriodCashFlowCalculatorTests : IClassFixture<CashFlowFixture<IMultiPeriodCashFlowCalculator>>
    {
        private readonly CashFlowFixture<IMultiPeriodCashFlowCalculator> _fixture;

        public MultiPeriodCashFlowCalculatorTests(CashFlowFixture<IMultiPeriodCashFlowCalculator> fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Salary and Wealth Projection")]
        public async Task Calculate_MultiPeriod_Simulation_With_Salary_And_Wealth()
        {
            // given
            int startingYear = 2021;
            int numberOfPeriods = 10;
            int municipalityId = 261; // Zurich
            Canton canton = Canton.ZH;
            MultiPeriodCalculatorPerson person = new()
            {
                CivilStatus = CivilStatus.Married,
                Name = "Unit Test",
                Canton = canton,
                MunicipalityId = municipalityId,
                Income = 100_000,
                Wealth = 500_000,
                NumberOfChildren = 0,
                PartnerReligiousGroupType = ReligiousGroupType.Other,
                ReligiousGroupType = ReligiousGroupType.Other
            };

            List<GenericCashFlowDefinition> cashFlowDefinitions = new();
            //{
            //    new GenericCashFlowDefinition
            //    {
            //        NetGrowthRate = 0,
            //        Name = "Test",
            //        InitialAmount = 6883,
            //        RecurringAmount = (6883, FrequencyType.Yearly),
            //        Flow = (AccountType.Income, AccountType.CapitalBenefits),
            //        InvestmentPeriod = (2021, 10),
            //        IsTaxable = true,
            //        TaxType = TaxType.Income
            //    }
            //};

            Dictionary<AccountType, decimal> initialFunds = new Dictionary<AccountType, decimal>();

            // when
            var result = await _fixture.Service.CalculateAsync(
                startingYear, numberOfPeriods, person, cashFlowDefinitions, initialFunds);

            // then
            Snapshot.Match(result);
        }
    }
}
