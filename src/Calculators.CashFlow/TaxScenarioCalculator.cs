using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calculators.CashFlow.Models;
using LanguageExt;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxComparison;
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

    public async Task<Either<string, CapitalBenefitsTransferInResult>> TransferInCapitalBenefitsAsync(
        int startingYear, int bfsMunicipalityId, TaxPerson person, TransferInCapitalBenefitsScenarioModel scenarioModel)
    {
        var birthdate = new DateTime(1969, 3, 17);

        MultiPeriodOptions options = new();
        options.CapitalBenefitsNetGrowthRate = scenarioModel.NetReturnRate;
        options.SavingsQuota = decimal.Zero;

        CashFlowDefinitionHolder cashFlowDefinitionHolder = CreateScenarioDefinitions();

        Either<string, MunicipalityModel> municipalityData =
            await municipalityResolver.GetAsync(bfsMunicipalityId, startingYear);

        Either<string, MultiPeriodCalculationResult> scenarioResult = await municipalityData
            .BindAsync(m =>
                multiPeriodCashFlowCalculator.CalculateAsync(startingYear, GetPerson(m, birthdate), cashFlowDefinitionHolder, options));

        CashFlowDefinitionHolder benchmarkDefinitions = CreateBenchmarkDefinitions();

        Either<string, MultiPeriodCalculationResult> benchmarkResult = await municipalityData
            .BindAsync(m =>
                multiPeriodCashFlowCalculator.CalculateAsync(startingYear, GetPerson(m, birthdate), benchmarkDefinitions, options));

        var benchmarkSeriesResult = benchmarkResult
            .Map(r => r.Accounts
                .Where(a => a.AccountType is AccountType.Wealth or AccountType.OccupationalPension));

        var scenarioSeriesResult = scenarioResult
            .Map(r => r.Accounts
                .Where(a => a.AccountType is AccountType.Wealth or AccountType.OccupationalPension));

        return from b in benchmarkSeriesResult
               from s in scenarioSeriesResult
               select CalculateDelta(b.ToList(), s.ToList());

        CapitalBenefitsTransferInResult CalculateDelta(
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
            
            return new CapitalBenefitsTransferInResult
            {
                StartingYear = Math.Min(benchmark.Min(a => a.Year), scenario.Min(a => a.Year)),
                NumberOfPeriods = deltaSeries.Count,
                BenchmarkSeries = benchmark.ToList(),
                ScenarioSeries = scenario.ToList(),
                DeltaSeries = deltaSeries
            };
        }
        
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

        MultiPeriodCalculatorPerson GetPerson(MunicipalityModel municipality, DateTime birthday)
        {
            return new MultiPeriodCalculatorPerson
            {
                CivilStatus = person.CivilStatus,
                DateOfBirth = birthday,
                Gender =  Gender.Male,
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

    private IEnumerable<ICashFlowDefinition> GetClearAccountAction(TransferInCapitalBenefitsScenarioModel scenarioModel)
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

    private static IEnumerable<ICompositeCashFlowDefinition> CreateTransferInDefinitions(TransferInCapitalBenefitsScenarioModel scenarioModel)
    {
        // one purchase transfer-in for each single transfer-in
        // as they might not be continuously
        foreach (var singleTransferIn in scenarioModel.TransferIns)
        {
            yield return new PurchaseInsuranceYearsPaymentsDefinition
            {
                DateOfStart = singleTransferIn.DateOfTransferIn,
                NetGrowthRate = scenarioModel.NetReturnRate,
                NumberOfInvestments = 1,
                YearlyAmount = singleTransferIn.Amount,
            };
        }
    }

    private static IEnumerable<ICompositeCashFlowDefinition> CreateDefaultComposites(
        TaxPerson person, TransferInCapitalBenefitsScenarioModel scenarioModel)
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
}
