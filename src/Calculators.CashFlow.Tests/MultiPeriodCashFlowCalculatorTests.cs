using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calculators.CashFlow.Models;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Actions;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;
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
            Snapshot.Match(result, opt => opt.IgnoreFields("$..Id"));
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
            Snapshot.Match(result, opt => opt.IgnoreFields("$..Id"));
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

            person.CapitalBenefits = (0, 500_000);

            // when
            var result = await _fixture.Service.CalculateAsync(
                startingYear,
                numberOfPeriods,
                person,
                new CashFlowDefinitionHolder
                {
                    GenericCashFlowDefinitions = GetCashFlowDefinitions().ToList(),
                    ChangeResidenceActions = GetChangeResidenceActions().ToList(),
                    CashFlowActions = GetCashFlowActions().ToList()
                },
                options);

            // then
            Snapshot.Match(result, opt => opt.IgnoreFields("$..Id"));
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
                Wealth = decimal.Zero,
                CapitalBenefits = (0, 100_000)
            };

            // when
            Either<string, MultiPeriodCalculationResult> result = await _fixture.Service.CalculateAsync(
                startingYear,
                numberOfPeriods,
                person,
                new CashFlowDefinitionHolder
                {
                    GenericCashFlowDefinitions = GetThirdPillarPaymentsDefinition().ToList(),
                },
                options);

            // then
            Snapshot.Match(result, opt => opt.IgnoreFields("$..Id"));
        }

        private static IEnumerable<GenericCashFlowDefinition> GetCashFlowDefinitions()
        {
            yield return new ThirdPillarPaymentsDefinition
                {
                    NetGrowthRate = decimal.Zero,
                    DateOfStart = new DateTime(2021, 1, 1),
                    NumberOfInvestments = 10,
                    YearlyAmount = 6883,
                }
                .CreateGenericDefinition();

            yield return new PensionPlanPaymentsDefinition
                {
                    DateOfStart = new DateTime(2021, 1, 1),
                    NetGrowthRate = 0,
                    YearlyAmount = 10000,
                    NumberOfInvestments = 5
                }
                .CreateGenericDefinition();
        }

        private static IEnumerable<GenericCashFlowDefinition> GetThirdPillarPaymentsDefinition()
        {
            yield return new ThirdPillarPaymentsDefinition
                {
                    DateOfStart = new DateTime(2021, 1, 1),
                    NetGrowthRate = 0.0M,
                    NumberOfInvestments = 10,
                    YearlyAmount = 6883,
                }
                .CreateGenericDefinition();
        }

        private MultiPeriodCalculatorPerson GetMarriedPerson(Canton canton, int municipalityId)
        {
            MultiPeriodCalculatorPerson person = new()
            {
                CivilStatus = CivilStatus.Married,
                DateOfBirth = new DateTime(1969, 3, 17),
                Gender = Gender.Male,
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

        private IEnumerable<ICashFlowDefinition> GetCashFlowActions()
        {
            yield return new OrdinaryRetirementAction
            {
                Header = new CashFlowHeader
                {
                    Id = "OrdinaryRetirementAction",
                    Name = "Ordinary Retirement"
                },
                NumberOfPeriods = 20,
                AhvPensionAmount = 29_400,
                CapitalConsumptionAmountPerYear = 20_000,
                CapitalOptionFactor = 0,
                RetirementPension = 34_000
            };
        }

        private IEnumerable<ChangeResidenceAction> GetChangeResidenceActions()
        {
            yield return new ChangeResidenceAction
            {
                Header = new CashFlowHeader
                {
                    Id = "ChangeResidence",
                    Name = "Change residence action"
                },
                DestinationMunicipalityId = 3426,
                DestinationCanton = Canton.SG,
                ChangeCost = 2_000,
                DateOfProcess = new DateTime(2029, 7, 1)
            };
        }
    }
}
