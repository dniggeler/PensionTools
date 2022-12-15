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

    public async Task<Either<string, MultiPeriodCalculationResult>> TransferInCapitalBenefitsAsync(
        int startingYear, int bfsMunicipalityId, TaxPerson person, TransferInCapitalBenefitsScenarioModel scenarioModel)
    {
        var birthdate = new DateTime(1969, 3, 17);

        MultiPeriodOptions options = new();
        options.CapitalBenefitsNetGrowthRate = scenarioModel.NetReturnRate;
        options.SavingsQuota = decimal.Zero;

        CashFlowDefinitionHolder cashFlowDefinitionHolder = CreateScenarioDefinitions();

        Either<string, MunicipalityModel> municipalityData =
            await municipalityResolver.GetAsync(bfsMunicipalityId, startingYear);

        Either<string, MultiPeriodCalculationResult> scenarioInResult = await municipalityData
            .BindAsync(m =>
                multiPeriodCashFlowCalculator.CalculateAsync(startingYear, GetPerson(m, birthdate), cashFlowDefinitionHolder, options));

        CashFlowDefinitionHolder benchmarkDefinitions = CreateBenchmarkDefinitions();

        Either<string, MultiPeriodCalculationResult> benchmarkResult = await municipalityData
            .BindAsync(m =>
                multiPeriodCashFlowCalculator.CalculateAsync(startingYear, GetPerson(m, birthdate), benchmarkDefinitions, options));

        return from b in benchmarkResult
            from s in scenarioInResult
            select CalculateDelta(b, s);

        MultiPeriodCalculationResult CalculateDelta(
            MultiPeriodCalculationResult benchmark,
            MultiPeriodCalculationResult scenario)
        {
            var diffResult = new MultiPeriodCalculationResult
            {
                StartingYear = benchmark.StartingYear,
                NumberOfPeriods = benchmark.NumberOfPeriods,
            };

            List<SinglePeriodCalculationResult> wealthAccount = new List<SinglePeriodCalculationResult>(benchmark.NumberOfPeriods);

            var zipped = benchmark.Accounts.Where(a => a.AccountType == AccountType.Wealth)
                .Zip(
                    scenario.Accounts.Where(a => a.AccountType == AccountType.Wealth),
                    (b, s) => (b.Year, Benchmark: b.Amount, Scenario: s.Amount));

            foreach (var zippedItem in zipped)
            {
                wealthAccount.Add(new SinglePeriodCalculationResult
                {
                    Year = zippedItem.Year,
                    Amount = zippedItem.Scenario - zippedItem.Benchmark,
                    AccountType = AccountType.Wealth
                });
            }

            diffResult.Accounts = wealthAccount;

            return diffResult;
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
            var capitalBenefits = scenarioModel.YearOfCapitalBenefitWithdrawal ?? 0;

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
                CapitalBenefits = (capitalBenefits, 0),
                NumberOfChildren = 0,
                PartnerReligiousGroupType = person.PartnerReligiousGroupType,
                ReligiousGroupType = person.ReligiousGroupType,
            };
        }
    }

    private IEnumerable<ICashFlowDefinition> GetClearAccountAction(TransferInCapitalBenefitsScenarioModel scenarioModel)
    {
        if (scenarioModel is { WithCapitalBenefitTaxation: false })
        {
            yield break;
        }

        int withdrawalYear = scenarioModel.YearOfCapitalBenefitWithdrawal ??
                             scenarioModel.TransferIns.Max(t => t.DateOfTransferIn.Year);
        DateTime withdrawalDate = new DateTime(withdrawalYear, 12, 31);

        yield return new DynamicTransferAccountAction
        {
            Header = new CashFlowHeader { Id = "ClearOccupationalPensionAccount", Name = "Clear Pension Account" },
            DateOfProcess = withdrawalDate,
            TransferRatio = decimal.One,
            Flow = new FlowPair(AccountType.OccupationalPension, AccountType.Wealth),
            IsTaxable = true,
            TaxType = TaxType.CapitalBenefits
        };
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

        DateTime finalDate = scenarioModel.WithCapitalBenefitTaxation
            ? new DateTime(scenarioModel.YearOfCapitalBenefitWithdrawal ?? finalSalaryPaymentDate.Year, 1, 1)
            : finalSalaryPaymentDate;

        yield return new SalaryPaymentsDefinition
        {
            YearlyAmount = person.TaxableIncome,
            DateOfEndOfPeriod = finalDate,
            NetGrowthRate = decimal.Zero,
        };

        yield return new SetupAccountDefinition
        {
            InitialOccupationalPensionAssets = scenarioModel.WithCapitalBenefitTaxation ? scenarioModel.FinalRetirementCapital : 0,
            InitialWealth = person.TaxableWealth
        };
    }
}
