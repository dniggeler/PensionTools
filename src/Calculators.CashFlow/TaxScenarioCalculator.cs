﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calculators.CashFlow.Models;
using LanguageExt;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Actions;

namespace Calculators.CashFlow;

public class TaxScenarioCalculator : ITaxScenarioCalculator
{
    private readonly IMultiPeriodCashFlowCalculator multiPeriodCashFlowCalculator;
    private readonly IMunicipalityConnector municipalityResolver;

    public TaxScenarioCalculator(
        IMultiPeriodCashFlowCalculator multiPeriodCashFlowCalculator, IMunicipalityConnector municipalityResolver)
    {
        this.multiPeriodCashFlowCalculator = multiPeriodCashFlowCalculator;
        this.municipalityResolver = municipalityResolver;
    }

    public async Task<Either<string, ScenarioCalculationResult>> CapitalBenefitTransferInsAsync(
        int startingYear, int bfsMunicipalityId, TaxPerson person, CapitalBenefitTransferInsScenarioModel scenarioModel)
    {
        var birthdate = new DateTime(1969, 3, 17);

        MultiPeriodOptions options = new();
        options.CapitalBenefitsNetGrowthRate = scenarioModel.NetReturnCapitalBenefits;
        options.WealthNetGrowthRate = scenarioModel.NetReturnWealth;
        options.SavingsQuota = decimal.Zero;

        CashFlowDefinitionHolder cashFlowDefinitionHolder = CreateScenarioDefinitions();

        Either<string, MunicipalityModel> municipalityData =
            await municipalityResolver.GetAsync(bfsMunicipalityId, startingYear);

        Either<string, MultiPeriodCalculationResult> scenarioResult = await municipalityData
            .BindAsync(m =>
                multiPeriodCashFlowCalculator.CalculateAsync(startingYear, 0, GetPerson(m, birthdate, person),
                    cashFlowDefinitionHolder, options));

        CashFlowDefinitionHolder benchmarkDefinitions = CreateBenchmarkDefinitions();

        Either<string, MultiPeriodCalculationResult> benchmarkResult = await municipalityData
            .BindAsync(m =>
                multiPeriodCashFlowCalculator.CalculateAsync(startingYear, 0, GetPerson(m, birthdate, person),
                    benchmarkDefinitions, options));

        var benchmarkSeriesResult = benchmarkResult
            .Map(r => r.Accounts
                .Where(a => a.AccountType is AccountType.Wealth or AccountType.OccupationalPension));

        var scenarioSeriesResult = scenarioResult
            .Map(r => r.Accounts
                .Where(a => a.AccountType is AccountType.Wealth or AccountType.OccupationalPension));

        return from b in benchmarkSeriesResult
               from s in scenarioSeriesResult
               select CalculateDelta(b.ToList(), s.ToList());

        CashFlowDefinitionHolder CreateBenchmarkDefinitions()
        {
            CashFlowDefinitionHolder holder = new CashFlowDefinitionHolder();
            holder.Composites = CreateDefaultComposites(person, scenarioModel).ToList();
            holder.CashFlowActions = GetClearAccountAction(scenarioModel).ToList();

            return holder;
        }

        CashFlowDefinitionHolder CreateScenarioDefinitions()
        {
            CashFlowDefinitionHolder holder = new CashFlowDefinitionHolder();
            holder.Composites = CreateTransferInDefinitions(scenarioModel)
                .Concat(CreateDefaultComposites(person, scenarioModel))
                .ToList();
            holder.CashFlowActions = GetClearAccountAction(scenarioModel).ToList();

            return holder;
        }
    }

    public async Task<Either<string, ScenarioCalculationResult>> ThirdPillarVersusSelfInvestmentAsync(
        int startingYear,
        int bfsMunicipalityId,
        TaxPerson person,
        ThirdPillarVersusSelfInvestmentScenarioModel scenarioModel)
    {
        var birthdate = new DateTime(1969, 3, 17);

        MultiPeriodOptions options = new();
        options.CapitalBenefitsNetGrowthRate = decimal.Zero;
        options.WealthNetGrowthRate = decimal.Zero;
        options.InvestmentNetGrowthRate = scenarioModel.InvestmentNetGrowthRate;
        options.SavingsQuota = decimal.Zero;

        CashFlowDefinitionHolder cashFlowDefinitionHolder = CreateScenarioDefinitions();

        Either<string, MunicipalityModel> municipalityData =
            await municipalityResolver.GetAsync(bfsMunicipalityId, startingYear);

        Either<string, MultiPeriodCalculationResult> scenarioResult = await municipalityData
            .BindAsync(m =>
                multiPeriodCashFlowCalculator.CalculateAsync(startingYear, 0, GetPerson(m, birthdate, person),
                    cashFlowDefinitionHolder, options));

        var scenarioSeriesResult = scenarioResult
            .Map(r => r.Accounts
                .Where(a => a.AccountType is AccountType.Wealth or AccountType.ThirdPillar));

        CashFlowDefinitionHolder benchmarkDefinitions = CreateBenchmarkDefinitions();

        Either<string, MultiPeriodCalculationResult> benchmarkResult = await municipalityData
            .BindAsync(m =>
                multiPeriodCashFlowCalculator.CalculateAsync(
                    startingYear,
                    scenarioModel.FinalYear - startingYear,
                    GetPerson(m, birthdate, person), benchmarkDefinitions, options));

        var benchmarkSeriesResult = benchmarkResult
            .Map(r => r.Accounts
                .Where(a => a.AccountType is AccountType.Wealth or AccountType.ThirdPillar or AccountType.Investment));

        return from b in benchmarkSeriesResult
            from s in scenarioSeriesResult
            select CalculateDeltaForThirdPillarComparison(b.ToList(), s.ToList());

        CashFlowDefinitionHolder CreateScenarioDefinitions()
        {
            CashFlowDefinitionHolder holder = new CashFlowDefinitionHolder();

            var thirdPillarInvestmentDefinitions = new List<ICompositeCashFlowDefinition>
            {
                new ThirdPillarPaymentsDefinition
                {
                    DateOfStart = new DateTime(startingYear, 1, 1),
                    NetGrowthRate = decimal.Zero,
                    NumberOfInvestments = scenarioModel.FinalYear - startingYear + 1,
                    YearlyAmount = scenarioModel.InvestmentAmount,
                }
            };

            var thirdPillarWithdrawalDefinitions = new List<ICashFlowDefinition>
            {
                new DynamicTransferAccountAction
                {
                    Header =
                        new CashFlowHeader {Id = Guid.NewGuid().ToString(), Name = "Third Pillar Withdrawal"},
                    DateOfProcess = new DateTime(scenarioModel.FinalYear, 12, 31),
                    TransferRatio = decimal.One,
                    Flow = new FlowPair(AccountType.ThirdPillar, AccountType.Wealth),
                    IsTaxable = true,
                    TaxType = TaxType.CapitalBenefits
                }
            };

            holder.Composites = thirdPillarInvestmentDefinitions
                .Concat(CreateSalaryPaymentDefinition(person, scenarioModel.FinalYear))
                .ToList();
            holder.CashFlowActions = thirdPillarWithdrawalDefinitions.ToList();

            return holder;
        }

        CashFlowDefinitionHolder CreateBenchmarkDefinitions()
        {
            CashFlowDefinitionHolder holder = new CashFlowDefinitionHolder();

            holder.InvestmentDefinitions = new List<InvestmentPortfolioDefinition>
            {
                new()
                {
                    Header = new CashFlowHeader {Id = Guid.NewGuid().ToString(), Name = "Self Investment"},
                    DateOfProcess = new DateTime(startingYear, 1, 1),
                    NetCapitalGrowthRate = scenarioModel.InvestmentNetGrowthRate,
                    NetIncomeRate = scenarioModel.InvestmentNetIncomeYield,
                    InitialInvestment = 0,
                    RecurringInvestment = new RecurringInvestment
                    {
                        Amount = scenarioModel.InvestmentAmount,
                        Frequency = FrequencyType.Yearly,
                    },
                    InvestmentPeriod = new InvestmentPeriod
                    {
                        Year = startingYear,
                        NumberOfPeriods = scenarioModel.FinalYear - startingYear + 1,
                    }
                }
            };

            holder.Composites = CreateSalaryPaymentDefinition(person, scenarioModel.FinalYear).ToList();

            return holder;
        }
    }

    private static IEnumerable<ICompositeCashFlowDefinition> CreateSalaryPaymentDefinition(
        TaxPerson person, int finalYear)
    {
        ICompositeCashFlowDefinition salaryPaymentDefinition = new SalaryPaymentsDefinition
        {
            YearlyAmount = person.TaxableIncome,
            DateOfEndOfPeriod = new DateTime(finalYear, 1, 1),
        };

        return new List<ICompositeCashFlowDefinition> { salaryPaymentDefinition };
    }

    private IEnumerable<ICashFlowDefinition> GetClearAccountAction(CapitalBenefitTransferInsScenarioModel scenarioModel)
    {
        if (scenarioModel is { WithCapitalBenefitWithdrawal: false })
        {
            yield break;
        }

        foreach (var withdrawal in scenarioModel.Withdrawals)
        {
            DateTime withdrawalDate = new DateTime(withdrawal.DateOfTransferIn.Year, 12, 31);

            yield return new DynamicTransferAccountAction
            {
                Header = new CashFlowHeader { Id = Guid.NewGuid().ToString(), Name = "Capital Benefit Withdrawal" },
                DateOfProcess = withdrawalDate,
                TransferRatio = withdrawal.Amount,
                Flow = new FlowPair(AccountType.OccupationalPension, AccountType.Wealth),
                IsTaxable = true,
                TaxType = TaxType.CapitalBenefits
            };
        }
    }

    private static IEnumerable<ICompositeCashFlowDefinition> CreateTransferInDefinitions(CapitalBenefitTransferInsScenarioModel scenarioModel)
    {
        // one purchase transfer-in for each single transfer-in
        // as they might not be continuously
        foreach (var singleTransferIn in scenarioModel.TransferIns)
        {
            yield return new PurchaseInsuranceYearsPaymentsDefinition
            {
                DateOfStart = singleTransferIn.DateOfTransferIn,
                NetGrowthRate = scenarioModel.NetReturnCapitalBenefits,
                NumberOfInvestments = 1,
                YearlyAmount = singleTransferIn.Amount,
            };
        }
    }

    private static IEnumerable<ICompositeCashFlowDefinition> CreateDefaultComposites(
        TaxPerson person, CapitalBenefitTransferInsScenarioModel scenarioModel)
    {
        DateTime finalSalaryPaymentDate = scenarioModel.TransferIns.Max(t => t.DateOfTransferIn).AddYears(1);

        DateTime finalDate = scenarioModel.WithCapitalBenefitWithdrawal
            ? scenarioModel.Withdrawals.Min(w => w.DateOfTransferIn)
            : finalSalaryPaymentDate;

        yield return new SalaryPaymentsDefinition
        {
            YearlyAmount = person.TaxableIncome,
            DateOfEndOfPeriod = finalDate,
            NetGrowthRate = decimal.Zero,
        };

        yield return new SetupAccountDefinition
        {
            InitialOccupationalPensionAssets = decimal.Zero,
            InitialWealth = person.TaxableWealth
        };

        yield return new FixedTransferAmountDefinition
        {
            DateOfProcess = new DateTime(finalDate.Year, 1, 1),
            Flow = new FlowPair(AccountType.Exogenous, AccountType.OccupationalPension),
            TransferAmount = scenarioModel.CapitalBenefitsBeforeWithdrawal,
            TaxType = TaxType.Undefined,
            IsTaxable = false,
        };
    }

    private static ScenarioCalculationResult CalculateDeltaForThirdPillarComparison(
        IReadOnlyCollection<SinglePeriodCalculationResult> benchmark,
        IReadOnlyCollection<SinglePeriodCalculationResult> scenario)
    {
        var sumBenchmarkSeries =
            from w in benchmark.Where(item => item.AccountType == AccountType.Wealth)
            from p in benchmark.Where(item => item.AccountType == AccountType.Investment)
            where w.Year == p.Year
            select new { Sum = w.Amount + p.Amount, w.Year };

        var sumScenarioSeries =
            from w in scenario.Where(item => item.AccountType == AccountType.Wealth)
            from p in scenario.Where(item => item.AccountType == AccountType.ThirdPillar)
            where w.Year == p.Year
            select new { Sum = w.Amount + p.Amount, w.Year };

        IEnumerable<SinglePeriodCalculationResult> deltaResults = from bSum in sumBenchmarkSeries
            from sSum in sumScenarioSeries
            where bSum.Year == sSum.Year
            select new SinglePeriodCalculationResult
            {
                Amount = sSum.Sum - bSum.Sum,
                Year = bSum.Year,
                AccountType = AccountType.Exogenous
            };

        List<SinglePeriodCalculationResult> deltaSeries = deltaResults.ToList();

        return new ScenarioCalculationResult
        {
            StartingYear = Math.Min(benchmark.Min(a => a.Year), scenario.Min(a => a.Year)),
            NumberOfPeriods = deltaSeries.Count,
            BenchmarkSeries = benchmark.ToList(),
            ScenarioSeries = scenario.ToList(),
            DeltaSeries = deltaSeries
        };
    }

    private static ScenarioCalculationResult CalculateDelta(
        IReadOnlyCollection<SinglePeriodCalculationResult> benchmark,
        IReadOnlyCollection<SinglePeriodCalculationResult> scenario)
    {
        var sumBenchmarkSeries =
            from w in benchmark.Where(item => item.AccountType == AccountType.Wealth)
            from p in benchmark.Where(item => item.AccountType == AccountType.OccupationalPension)
            where w.Year == p.Year
            select new { Sum = w.Amount + p.Amount, w.Year };

        var sumScenarioSeries =
            from w in scenario.Where(item => item.AccountType == AccountType.Wealth)
            from p in scenario.Where(item => item.AccountType == AccountType.OccupationalPension)
            where w.Year == p.Year
            select new { Sum = w.Amount + p.Amount, w.Year };

        IEnumerable<SinglePeriodCalculationResult> deltaResults = from bSum in sumBenchmarkSeries
            from sSum in sumScenarioSeries
            where bSum.Year == sSum.Year
            select new SinglePeriodCalculationResult
            {
                Amount = sSum.Sum - bSum.Sum,
                Year = bSum.Year,
                AccountType = AccountType.Exogenous
            };

        List<SinglePeriodCalculationResult> deltaSeries = deltaResults.ToList();

        return new ScenarioCalculationResult
        {
            StartingYear = Math.Min(benchmark.Min(a => a.Year), scenario.Min(a => a.Year)),
            NumberOfPeriods = deltaSeries.Count,
            BenchmarkSeries = benchmark.ToList(),
            ScenarioSeries = scenario.ToList(),
            DeltaSeries = deltaSeries
        };
    }

    private static MultiPeriodCalculatorPerson GetPerson(MunicipalityModel municipality, DateTime birthday, TaxPerson person)
    {
        return new MultiPeriodCalculatorPerson
        {
            CivilStatus = person.CivilStatus,
            DateOfBirth = birthday,
            Gender = Gender.Male,
            Name = "Purchase Scenario",
            Canton = municipality.Canton,
            MunicipalityId = municipality.BfsNumber,
            Income = person.TaxableIncome,
            Wealth = person.TaxableWealth,
            CapitalBenefits = (0, 0),
            NumberOfChildren = 0,
            PartnerReligiousGroupType = person.PartnerReligiousGroupType,
            ReligiousGroupType = person.ReligiousGroupType,
        };
    }
}
