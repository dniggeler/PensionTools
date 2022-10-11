using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calculators.CashFlow.Models;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Actions;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;
using PensionCoach.Tools.CommonTypes.Tax;
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
            MultiPeriodOptions options = new();
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
                new CashFlowDefinitionHolder(),
                options);

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

            MultiPeriodOptions options = new();
            options.WealthNetGrowthRate = 0.01M;

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
                new CashFlowDefinitionHolder(),
                options);

            // then
            Snapshot.Match(result);
        }

        [Fact(DisplayName = "Full Simulation")]
        public async Task Calculate_Full_MultiPeriod_Simulation()
        {
            // given
            int startingYear = 2021;
            int numberOfPeriods = 10;
            int municipalityId = 261;
            Canton canton = Canton.ZH;
            MultiPeriodOptions options = new();
            MultiPeriodCalculatorPerson person = GetMarriedPerson(canton, municipalityId);

            // when
            var result = await _fixture.Service.CalculateAsync(
                startingYear,
                numberOfPeriods,
                person,
                new CashFlowDefinitionHolder
                {
                    GenericCashFlowDefinitions = GetCashFlowDefinitions(person).ToList(),
                    ClearAccountActions = GetClearActionDefinitions().ToList(),
                    ChangeResidenceActions = GetChangeResidenceActions().ToList(),
                },
                options);

            // then
            Snapshot.Match(result);
        }

        [Fact(DisplayName = "3a Simulation")]
        public async Task Calculate_ThirdPillar_Simulation()
        {
            // given
            int startingYear = 2021;
            int numberOfPeriods = 10;
            int municipalityId = 261;
            Canton canton = Canton.ZH;

            MultiPeriodOptions options = new();
            options.SalaryNetGrowthRate = decimal.Zero;

            MultiPeriodCalculatorPerson person = GetMarriedPerson(canton, municipalityId) with
            {
                Wealth = decimal.Zero
            };

            // when
            var result = await _fixture.Service.CalculateAsync(
                startingYear,
                numberOfPeriods,
                person,
                new CashFlowDefinitionHolder
                {
                    GenericCashFlowDefinitions = GetThirdPillarCashFlowDefinitions(person).ToList(),
                },
                options);

            // then
            Snapshot.Match(result);
        }

        private static IEnumerable<GenericCashFlowDefinition> GetCashFlowDefinitions(MultiPeriodCalculatorPerson person)
        {
            List<GenericCashFlowDefinition> definitions = new SetupAccountDefinition
                {
                    DateOfStart = new DateTime(2021, 1, 1),
                    InitialWealth = 0,
                    InitialCapitalBenefits = 500_000,
                }
                .CreateGenericDefinition()
                .ToList();

            definitions.Add(new ThirdPillarPaymentsDefinition
            {
                Header = new CashFlowHeader
                {
                    Id = "my 3a account",
                    Name = $"{person.Name} - 3a Pillar",
                },
                NetGrowthRate = decimal.Zero,
                DateOfStart = new DateTime(2021, 1, 1),
                NumberOfInvestments = 10,
                YearlyAmount = 6883,
            }.CreateGenericDefinition());

            definitions.Add(new PensionPlanPaymentsDefinition
                {
                    Header = new CashFlowHeader
                    {
                        Id = "my PK account",
                        Name = "PK-Einkauf",
                    },
                    DateOfStart = new DateTime(2021, 1, 1),
                    NetGrowthRate = 0,
                    YearlyAmount = 10000,
                    NumberOfInvestments = 5
                }
                .CreateGenericDefinition());

            return definitions;
        }

        private static IEnumerable<GenericCashFlowDefinition> GetThirdPillarCashFlowDefinitions(MultiPeriodCalculatorPerson person)
        {
            List<GenericCashFlowDefinition> definitions = new SetupAccountDefinition
                {
                    DateOfStart = new DateTime(2021, 1, 1),
                    InitialWealth = decimal.Zero,
                    InitialCapitalBenefits = 100_000
                }
                .CreateGenericDefinition()
                .ToList();

            definitions.Add(new ThirdPillarPaymentsDefinition
                {
                    Header = new CashFlowHeader
                    {
                        Id = "my 3a account",
                        Name = $"{person.Name} - 3a Pillar"
                    },
                    DateOfStart = new DateTime(2021, 1, 1),
                    NetGrowthRate = 0.0M,
                    NumberOfInvestments = 10,
                    YearlyAmount = 6883,
                }
                .CreateGenericDefinition());

            return definitions;
        }

        private MultiPeriodCalculatorPerson GetMarriedPerson(Canton canton, int municipalityId)
        {
            MultiPeriodCalculatorPerson person = new()
            {
                CivilStatus = CivilStatus.Married,
                Name = "Unit Test",
                Canton = canton,
                MunicipalityId = municipalityId,
                Income = 100_000,
                Wealth = 500_000,
                CapitalBenefits = (0, 0),
                NumberOfChildren = 0,
                PartnerReligiousGroupType = ReligiousGroupType.Other,
                ReligiousGroupType = ReligiousGroupType.Other
            };

            return person;
        }

        private IEnumerable<ClearAccountAction> GetClearActionDefinitions()
        {
            yield return new ClearAccountAction
            {
                Id = "Clear Capital Benefit Action 1",
                ClearAtYear = 2030,
                ClearRatio = 1.0M,
                Flow = new FlowPair(AccountType.CapitalBenefits, AccountType.Wealth),
                IsTaxable = true,
                TaxType = TaxType.Capital,
                OccurrenceType = OccurrenceType.EndOfPeriod,
            };
        }

        private IEnumerable<ChangeResidenceAction> GetChangeResidenceActions()
        {
            yield return new ChangeResidenceAction
            {
                Id = "Change residence action",
                DestinationMunicipalityId = 3426,
                DestinationCanton = Canton.SG,
                ChangeCost = 2_000,
                ChangeAtYear = 2029,
            };
        }
    }
}
