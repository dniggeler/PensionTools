using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Extensions;
using Application.MultiPeriodCalculator;
using Domain.Contracts;
using Domain.Enums;
using Domain.Models.MultiPeriod;
using Domain.Models.MultiPeriod.Actions;
using Domain.Models.MultiPeriod.Definitions;
using LanguageExt;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
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
            int numberOfPeriods = 0;
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

        [Fact(DisplayName = "Investment Portfolio Simulation")]
        public async Task Calculate_Investment_Portfolio()
        {
            // given
            int startingYear = 2021;
            int numberOfPeriods = 0;
            int municipalityId = 261;
            Canton canton = Canton.ZH;
            decimal initialInvestmentAmount = 93277;
            MultiPeriodOptions options = new();
            MultiPeriodCalculatorPerson person = GetMarriedPerson(canton, municipalityId) with
            {
                Income = 100_000,
                Wealth = 500_000,
                CapitalBenefits = (decimal.Zero, decimal.Zero)
            };

            // when
            var result = await _fixture.Service.CalculateAsync(
                startingYear,
                numberOfPeriods,
                person,
                new CashFlowDefinitionHolder
                {
                    InvestmentDefinitions = CreateInvestmentPortfolios().ToList(),
                    Composites = CreateComposites(person, initialInvestmentAmount).ToList()
                },
                options);

            // then
            Snapshot.Match(result, opt => opt.IgnoreFields("$..Id"));

            static IEnumerable<InvestmentPortfolioDefinition> CreateInvestmentPortfolios()
            {
                yield return new InvestmentPortfolioDefinition
                {
                    Header = new CashFlowHeader()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Investment Portfolio",
                    },
                    DateOfProcess = new DateTime(2021, 1, 1),
                    NetCapitalGrowthRate = 0.02M,
                    NetIncomeRate = 0.01M,
                    RecurringInvestment = new RecurringInvestment
                    {
                        Amount = 6723,
                        Frequency = FrequencyType.Yearly,
                    },
                    InvestmentPeriod = new InvestmentPeriod
                    {
                        Year = 2021,
                        NumberOfPeriods = 10
                    },
                };
            }

            static IEnumerable<ICompositeCashFlowDefinition> CreateComposites(
                MultiPeriodCalculatorPerson person, decimal initialInvestmentAmount)
            {
                yield return new SalaryPaymentsDefinition
                {
                    YearlyAmount = person.Income,
                    DateOfEndOfPeriod = new DateTime(2021, 1, 1).AddYears(10)
                };

                yield return new SetupAccountDefinition
                {
                    InitialWealth = person.Wealth,
                    InitialInvestmentAssets = initialInvestmentAmount
                };
            }
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

            var cashFlowDefinitionHolder = new CashFlowDefinitionHolder()
            {
                Composites = CreateSetupAccountComposites(person).ToList()
            };

            // when
            var result = await _fixture.Service.CalculateAsync(
                startingYear,
                numberOfPeriods,
                person,
                cashFlowDefinitionHolder,
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
                    ChangeResidenceActions = GetChangeResidenceActions().ToList(),
                    Composites = GetComposites(person).Concat(CreateSetupAccountComposites(person)).ToList()
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
                Wealth = 10_000,
                CapitalBenefits = (0, 100_000)
            };

            // when
            Either<string, MultiPeriodCalculationResult> result = await _fixture.Service.CalculateAsync(
                startingYear,
                numberOfPeriods,
                person,
                new CashFlowDefinitionHolder
                {
                    Composites  = GetThirdPillarPaymentsDefinition().Concat(CreateSetupAccountComposites(person)).ToList(),
                },
                options);

            // then
            Snapshot.Match(result, opt => opt.IgnoreFields("$..Id"));
        }

        private static IEnumerable<ICompositeCashFlowDefinition> GetThirdPillarPaymentsDefinition()
        {
            yield return new ThirdPillarPaymentsDefinition
            {
                DateOfStart = new DateTime(2021, 1, 1),
                NetGrowthRate = 0.0M,
                NumberOfInvestments = 10,
                YearlyAmount = 6883,
            };

            yield return new SalaryPaymentsDefinition
            {
                YearlyAmount = 100_000, DateOfEndOfPeriod = new DateTime(2033, 1, 1)
            };
        }

        private static IEnumerable<ICompositeCashFlowDefinition> CreateSetupAccountComposites(MultiPeriodCalculatorPerson person)
        {
            ICompositeCashFlowDefinition accountSetupDefinition = new SetupAccountDefinition
            {
                InitialOccupationalPensionAssets = person.CapitalBenefits.PensionPlan + person.CapitalBenefits.Pillar3a,
                InitialWealth = person.Wealth
            };

            return new List<ICompositeCashFlowDefinition> { accountSetupDefinition };
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

        private IEnumerable<ICompositeCashFlowDefinition> GetComposites(MultiPeriodCalculatorPerson person)
        {
            DateTime startDate = new DateTime(2021, 1, 1);
            DateTime retirementDate = person.DateOfBirth.GetRetirementDate(person.Gender);

            yield return new SalaryPaymentsDefinition
            {
                YearlyAmount = person.Income,
                DateOfEndOfPeriod = retirementDate,
                NetGrowthRate = 0.01M,
            };

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

            yield return new ThirdPillarPaymentsDefinition
            {
                NetGrowthRate = decimal.Zero,
                DateOfStart = startDate,
                NumberOfInvestments = 10,
                YearlyAmount = 6883,
            };

            yield return new PurchaseInsuranceYearsPaymentsDefinition
            {
                DateOfStart = startDate,
                NetGrowthRate = 0,
                YearlyAmount = 10000,
                NumberOfInvestments = 5
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
