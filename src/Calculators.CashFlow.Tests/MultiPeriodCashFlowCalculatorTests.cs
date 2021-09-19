using System.Collections.Generic;
using System.Linq;
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

        [Fact(DisplayName = "Zero Simulation")]
        public async Task Calculate_Zero_Simulation()
        {
            // given
            int startingYear = 2021;
            int numberOfPeriods = 10;
            int municipalityId = 261;
            Canton canton = Canton.ZH;
            MultiPeriodCalculatorPerson person = GetMarriedPerson(canton, municipalityId) with
            {
                Income = decimal.Zero,
                Wealth = decimal.Zero,
                CapitalBenefits = (decimal.Zero, decimal.Zero)
            };

            // when
            var result = await _fixture.Service.CalculateAsync(
                startingYear,
                numberOfPeriods,
                person,
                new CashFlowDefinitionHolder());

            // then
            Snapshot.Match(result);
        }

        [Fact(DisplayName = "Wealth Only Simulation")]
        public async Task Calculate_Wealth_Only_Simulation()
        {
            // given
            int startingYear = 2021;
            int numberOfPeriods = 10;
            int municipalityId = 261;
            Canton canton = Canton.ZH;
            MultiPeriodCalculatorPerson person = GetMarriedPerson(canton, municipalityId) with
            {
                Income = decimal.Zero,
                Wealth = 500_000,
                CapitalBenefits = (decimal.Zero, decimal.Zero)
            };

            // when
            var result = await _fixture.Service.CalculateAsync(
                startingYear,
                numberOfPeriods,
                person,
                new CashFlowDefinitionHolder());

            // then
            Snapshot.Match(result);
        }

        [Fact(DisplayName = "Salary and Wealth Projection")]
        public async Task Calculate_MultiPeriod_Simulation_With_Salary_And_Wealth()
        {
            // given
            int startingYear = 2021;
            int numberOfPeriods = 10;
            int municipalityId = 261;
            Canton canton = Canton.ZH;
            MultiPeriodCalculatorPerson person = GetMarriedPerson(canton, municipalityId);

            // when
            var result = await _fixture.Service.CalculateAsync(
                startingYear,
                numberOfPeriods,
                person,
                new CashFlowDefinitionHolder
                {
                    GenericCashFlowDefinitions = GetCashFlowDefinitions(person).ToList(),
                    ClearCashFlowDefinitions = GetClearActionDefinitions().ToList()
                });

            // then
            Snapshot.Match(result);
        }

        private static IEnumerable<GenericCashFlowDefinition> GetCashFlowDefinitions(MultiPeriodCalculatorPerson person)
        {
            yield return new GenericCashFlowDefinition
            {
                Id = "my 3a account",
                Name = $"{person.Name} - 3a Pillar",
                InitialAmount = 6883,
                RecurringInvestment = new RecurringInvestment(6883, FrequencyType.Yearly),
                Flow = new FlowPair(AccountType.Income, AccountType.CapitalBenefits),
                InvestmentPeriod = new InvestmentPeriod(2021, 10),
                IsTaxable = false,
                TaxType = TaxType.Undefined,
                OccurrenceType = OccurrenceType.BeginOfPeriod
            };

            yield return new GenericCashFlowDefinition
            {
                Id = "my PK account",
                NetGrowthRate = 0,
                Name = "PK-Einkauf",
                InitialAmount = 10000,
                RecurringInvestment = new RecurringInvestment(10000, FrequencyType.Yearly),
                Flow = new FlowPair(AccountType.Income, AccountType.CapitalBenefits),
                InvestmentPeriod = new InvestmentPeriod(2021, 5),
                IsTaxable = false,
                TaxType = TaxType.Undefined,
                OccurrenceType = OccurrenceType.BeginOfPeriod
            };
        }

        MultiPeriodCalculatorPerson GetMarriedPerson(Canton canton, int municipalityId)
        {
            MultiPeriodCalculatorPerson person = new()
            {
                CivilStatus = CivilStatus.Married,
                Name = "Unit Test",
                Canton = canton,
                MunicipalityId = municipalityId,
                Income = 100_000,
                Wealth = 500_000,
                CapitalBenefits = (100_000, 400_000),
                NumberOfChildren = 0,
                PartnerReligiousGroupType = ReligiousGroupType.Other,
                ReligiousGroupType = ReligiousGroupType.Other
            };

            return person;
        }

        IEnumerable<ClearActionDefinition> GetClearActionDefinitions()
        {
            yield return new ClearActionDefinition
            {
                Id = "Clear Capital Benefit Action 1",
                ClearAtYear = 2029,
                ClearRatio = 0.5M,
                Flow = new FlowPair(AccountType.CapitalBenefits, AccountType.Wealth),
                IsTaxable = true,
                TaxType = TaxType.Capital,
                OccurrenceType = OccurrenceType.EndOfPeriod,
            };

            yield return new ClearActionDefinition
            {
                Id = "Clear Capital Benefit Action 2",
                ClearAtYear = 2030,
                ClearRatio = 1.0M,
                Flow = new FlowPair(AccountType.CapitalBenefits, AccountType.Wealth),
                IsTaxable = true,
                TaxType = TaxType.Capital,
                OccurrenceType = OccurrenceType.EndOfPeriod,
            };
        }
    }
}
