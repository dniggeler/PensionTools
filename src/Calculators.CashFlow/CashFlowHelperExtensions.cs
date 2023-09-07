using System;
using System.Collections.Generic;
using System.Linq;
using Calculators.CashFlow.Accounts;
using Calculators.CashFlow.Models;
using PensionCoach.Tools.BvgCalculator;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Actions;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.CommonUtils;

namespace Calculators.CashFlow;

public static class CashFlowHelperExtensions
{
    public static IEnumerable<CashFlowModel> GenerateCashFlow(this IStaticCashFlowDefinition definition)
    {
        return definition switch
        {
            StaticGenericCashFlowDefinition d => d.GenerateCashFlow(),
            _ => Array.Empty<CashFlowModel>()
        };
    }

    public static IEnumerable<CashFlowModel> GenerateCashFlow(this StaticGenericCashFlowDefinition definition)
    {
        var range = Enumerable.Range(
            definition.InvestmentPeriod.Year,
            definition.InvestmentPeriod.NumberOfPeriods);

        yield return new CashFlowModel(
            DateOnly.FromDateTime(definition.DateOfProcess), 
            definition.InitialAmount,
            AccountType.Exogenous,
            definition.Flow.Target,
            definition.IsTaxable,
            definition.TaxType);

        decimal cashFlow = definition.RecurringInvestment.Amount;

        var currentDate = DateOnly.FromDateTime(definition.DateOfProcess);
        foreach (var year in range)
        {
            yield return new CashFlowModel(
                currentDate.AddYears(year - definition.DateOfProcess.Year),
                cashFlow,
                definition.Flow.Source,
                definition.Flow.Target,
                definition.IsTaxable,
                definition.TaxType);

            cashFlow *= decimal.One + definition.NetGrowthRate;
        }
    }

    public static IEnumerable<ICashFlowDefinition> CreateGenericDefinition(this FixedTransferAmountDefinition action)
    {
        yield return new StaticGenericCashFlowDefinition
        {
            Header = new CashFlowHeader { Id = "transferAmount", Name = "Transfer Fixed Amount" },
            DateOfProcess = action.DateOfProcess,
            InitialAmount = decimal.Zero,
            NetGrowthRate = decimal.Zero,
            Flow = new FlowPair(action.Flow.Source, action.Flow.Target),
            RecurringInvestment = new RecurringInvestment
            {
                Amount = action.TransferAmount,
                Frequency = FrequencyType.Yearly,
            },
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = action.DateOfProcess.Year,
                NumberOfPeriods = 1,
            },
            IsTaxable = action.IsTaxable,
            TaxType = action.TaxType
        };
    }

    public static IEnumerable<IStaticCashFlowDefinition> CreateGenericDefinition(
        this DynamicTransferAccountAction action, Dictionary<AccountType, ICashFlowAccount> accounts)
    {
        decimal transferAmount = accounts[action.Flow.Source].Balance * action.TransferRatio;

        yield return new StaticGenericCashFlowDefinition
        {
            Header = new CashFlowHeader { Id = "transferAmount", Name = "Transfer Amount By Ratio" },
            DateOfProcess = action.DateOfProcess,
            InitialAmount = decimal.Zero,
            NetGrowthRate = decimal.Zero,
            Flow = new FlowPair(action.Flow.Source, action.Flow.Target),
            RecurringInvestment = new RecurringInvestment
            {
                Amount = transferAmount,
                Frequency = FrequencyType.Yearly,
            },
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = action.DateOfProcess.Year,
                NumberOfPeriods = 1,
            },
            IsTaxable = action.IsTaxable,
            TaxType = action.TaxType
        };
    }

    public static IEnumerable<ICashFlowDefinition> CreateGenericDefinition(
        this SetupAccountDefinition accountDefinition, DateTime dateOfStart)
    {
        yield return new StaticGenericCashFlowDefinition
        {
            Header = new CashFlowHeader
            {
                Id = "setupWealthAccount",
                Name = "Wealth Account"
            },
            DateOfProcess = dateOfStart,
            InitialAmount = accountDefinition.InitialWealth,
            Flow = new FlowPair(AccountType.Exogenous, AccountType.Wealth),
            RecurringInvestment = new RecurringInvestment
            {
                Amount = 0,
                Frequency = FrequencyType.Yearly,
            },
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = dateOfStart.Year,
                NumberOfPeriods = 0
            },
            IsTaxable = true,
            TaxType = TaxType.Wealth
        };

        yield return new StaticGenericCashFlowDefinition
        {
            Header = new CashFlowHeader
            {
                Id = "setupOccupationalPensionAccount",
                Name = "Occupational Pension Account"
            },
            DateOfProcess = dateOfStart,
            InitialAmount = accountDefinition.InitialOccupationalPensionAssets,
            Flow = new FlowPair(AccountType.Exogenous, AccountType.OccupationalPension),
            RecurringInvestment = new RecurringInvestment
            {
                Amount = 0,
                Frequency = FrequencyType.Yearly,
            },
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = dateOfStart.Year,
                NumberOfPeriods = 0
            },
            IsTaxable = false,
            TaxType = TaxType.CapitalBenefits
        };

        yield return new StaticGenericCashFlowDefinition
        {
            Header = new CashFlowHeader
            {
                Id = "setupThirdPillarAccount",
                Name = "Third Pillar Account"
            },
            DateOfProcess = dateOfStart,
            InitialAmount = accountDefinition.InitialThirdPillarAssets,
            Flow = new FlowPair(AccountType.Exogenous, AccountType.ThirdPillar),
            RecurringInvestment = new RecurringInvestment
            {
                Amount = 0,
                Frequency = FrequencyType.Yearly,
            },
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = dateOfStart.Year,
                NumberOfPeriods = 0
            },
            IsTaxable = false,
            TaxType = TaxType.CapitalBenefits
        };

        yield return new StaticGenericCashFlowDefinition
        {
            Header = new CashFlowHeader
            {
                Id = "setupInvestmentAccount",
                Name = "Investment Account"
            },
            DateOfProcess = dateOfStart,
            InitialAmount = accountDefinition.InitialInvestmentAssets,
            Flow = new FlowPair(AccountType.Exogenous, AccountType.Investment),
            RecurringInvestment = new RecurringInvestment
            {
                Amount = 0,
                Frequency = FrequencyType.Yearly,
            },
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = dateOfStart.Year,
                NumberOfPeriods = 0
            },
        };
    }

    public static IEnumerable<ICashFlowDefinition> CreateGenericDefinition(
        this PurchaseInsuranceYearsPaymentsDefinition purchaseDefinition)
    {
        yield return new StaticGenericCashFlowDefinition
        {
            Header = new CashFlowHeader
            {
                Id = "pensionPlanPayments",
                Name = "Pension Plan Payments"
            },
            DateOfProcess = purchaseDefinition.DateOfStart,
            InitialAmount = decimal.Zero,
            NetGrowthRate = purchaseDefinition.NetGrowthRate,
            Flow = new FlowPair(AccountType.Income, AccountType.OccupationalPension),
            RecurringInvestment = new RecurringInvestment
            {
                Amount = purchaseDefinition.YearlyAmount,
                Frequency = FrequencyType.Yearly,
            },
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = purchaseDefinition.DateOfStart.Year,
                NumberOfPeriods = purchaseDefinition.NumberOfInvestments,
            },
            IsTaxable = false,
            TaxType = TaxType.Undefined
        };

        yield return new StaticGenericCashFlowDefinition
        {
            Header = new CashFlowHeader
            {
                Id = "purchaseFundingByWealth",
                Name = "Funding of Purchase by Wealth"
            },
            DateOfProcess = purchaseDefinition.DateOfStart,
            InitialAmount = decimal.Zero,
            NetGrowthRate = purchaseDefinition.NetGrowthRate,
            Flow = new FlowPair(AccountType.Wealth, AccountType.Exogenous),
            RecurringInvestment = new RecurringInvestment
            {
                Amount = purchaseDefinition.YearlyAmount,
                Frequency = FrequencyType.Yearly,
            },
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = purchaseDefinition.DateOfStart.Year,
                NumberOfPeriods = purchaseDefinition.NumberOfInvestments,
            },
            IsTaxable = false,
            TaxType = TaxType.Undefined
        };
    }

    public static IEnumerable<ICashFlowDefinition> CreateGenericDefinition(
        this SalaryPaymentsDefinition salaryDefinition, DateTime dateOfStart)
    {
        int numberOfInvestments = salaryDefinition.DateOfEndOfPeriod.Year - dateOfStart.Year;

        // payments for all periods except the last one
        yield return new StaticGenericCashFlowDefinition
        {
            Header = new CashFlowHeader
            {
                Id = "SalaryPaymentsDefinition",
                Name = "SalaryPaymentsDefinition"
            },
            DateOfProcess = dateOfStart,
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = dateOfStart.Year,
                NumberOfPeriods = numberOfInvestments,
            },
            Flow = new FlowPair(AccountType.Exogenous, AccountType.Income),
            InitialAmount = decimal.Zero,
            NetGrowthRate = salaryDefinition.NetGrowthRate,
            RecurringInvestment = new RecurringInvestment
            {
                Amount = salaryDefinition.YearlyAmount,
                Frequency = FrequencyType.Yearly,
            },
            IsTaxable = true,
            TaxType = TaxType.Income
        };

        // last period (might be pro-rated)
        DateTime dateOfBeginYear = salaryDefinition.DateOfEndOfPeriod.BeginOfYearDate();
        decimal lastPeriodPayment = salaryDefinition.YearlyAmount *
                                    (salaryDefinition.DateOfEndOfPeriod.GetYears360() -
                                     dateOfBeginYear.GetYears360());

        yield return new StaticGenericCashFlowDefinition
        {
            Header = new CashFlowHeader
            {
                Id = "SalaryPaymentsDefinition",
                Name = "SalaryPaymentsDefinition"
            },
            DateOfProcess = dateOfBeginYear,
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = dateOfStart.Year,
                NumberOfPeriods = 0
            },
            Flow = new FlowPair(AccountType.Exogenous, AccountType.Income),
            InitialAmount = lastPeriodPayment,
            NetGrowthRate = decimal.Zero,
            RecurringInvestment = new RecurringInvestment
            {
                Amount = decimal.Zero,
                Frequency = FrequencyType.Yearly
            },
            IsTaxable = true,
            TaxType = TaxType.Income
        };
    }

    public static IEnumerable<IStaticCashFlowDefinition> CreateGenericDefinition(this ThirdPillarPaymentsDefinition thirdPillarDefinition)
    {
        yield return new StaticGenericCashFlowDefinition
        {
            Header = new CashFlowHeader
            {
                Id = "thirdPillarPayments",
                Name = "3a Payments"
            },
            DateOfProcess = thirdPillarDefinition.DateOfStart,
            InitialAmount = decimal.Zero,
            NetGrowthRate = thirdPillarDefinition.NetGrowthRate,
            Flow = new FlowPair(AccountType.Income, AccountType.ThirdPillar),
            RecurringInvestment = new RecurringInvestment
            {
                Amount = thirdPillarDefinition.YearlyAmount,
                Frequency = FrequencyType.Yearly,
            },
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = thirdPillarDefinition.DateOfStart.Year,
                NumberOfPeriods = thirdPillarDefinition.NumberOfInvestments,
            },
            IsTaxable = false,
            TaxType = TaxType.Undefined
        };

        yield return new StaticGenericCashFlowDefinition
        {
            Header = new CashFlowHeader
            {
                Id = "thirdPillarPaymentsFunding",
                Name = "3a Payments Funded by Wealth"
            },
            DateOfProcess = thirdPillarDefinition.DateOfStart,
            InitialAmount = decimal.Zero,
            NetGrowthRate = thirdPillarDefinition.NetGrowthRate,
            Flow = new FlowPair(AccountType.Wealth, AccountType.Exogenous),
            RecurringInvestment = new RecurringInvestment
            {
                Amount = thirdPillarDefinition.YearlyAmount,
                Frequency = FrequencyType.Yearly,
            },
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = thirdPillarDefinition.DateOfStart.Year,
                NumberOfPeriods = thirdPillarDefinition.NumberOfInvestments,
            },
            IsTaxable = false,
            TaxType = TaxType.Undefined
        };
    }

    public static IEnumerable<IStaticCashFlowDefinition> CreateGenericDefinition(this InvestmentPortfolioDefinition investmentPortfolioDefinition)
    {
        yield return new StaticGenericCashFlowDefinition
        {
            Header = investmentPortfolioDefinition.Header,
            DateOfProcess = investmentPortfolioDefinition.DateOfProcess,
            InitialAmount = decimal.Zero,
            NetGrowthRate = investmentPortfolioDefinition.NetCapitalGrowthRate,
            RecurringInvestment = investmentPortfolioDefinition.RecurringInvestment,
            InvestmentPeriod = investmentPortfolioDefinition.InvestmentPeriod,
            Flow = new FlowPair(AccountType.Wealth, AccountType.Investment),
        };
    }

    public static IEnumerable<ICashFlowDefinition> CreateGenericDefinition(
        this ICompositeCashFlowDefinition cashFlowDefinition, MultiPeriodCalculatorPerson person, DateTime dateOfStart)
    {
        return cashFlowDefinition switch
        {
            OrdinaryRetirementAction a => a.CreateCashFlows(person),
            _ => CreateGenericDefinition(cashFlowDefinition, dateOfStart)
        };
    }

    public static IEnumerable<ICashFlowDefinition> CreateGenericDefinition(
        this ICompositeCashFlowDefinition cashFlowDefinition, DateTime dateOfStart)
    {
        return cashFlowDefinition switch
        {
            SetupAccountDefinition s => s.CreateGenericDefinition(dateOfStart),
            SalaryPaymentsDefinition p => p.CreateGenericDefinition(dateOfStart),
            FixedTransferAmountDefinition t => t.CreateGenericDefinition(),
            ThirdPillarPaymentsDefinition p => p.CreateGenericDefinition(),
            PurchaseInsuranceYearsPaymentsDefinition y => y.CreateGenericDefinition(),
            _ => Array.Empty<StaticGenericCashFlowDefinition>()
        };
    }

    public static IEnumerable<IStaticCashFlowDefinition> CreateGenericDefinition(
        this IDynamicCashFlowDefinition dynamicCashFlowDefinition, Dictionary<AccountType, ICashFlowAccount> accounts)
    {
        return dynamicCashFlowDefinition switch
        {
            DynamicTransferAccountAction t => t.CreateGenericDefinition(accounts),

            _ => Array.Empty<StaticGenericCashFlowDefinition>()
        };
    }

    public static IEnumerable<CashFlowModel> AggregateCashFlows(this IEnumerable<CashFlowModel> cashFlows)
    {
        return cashFlows
            .GroupBy(keySelector => new
            {
                DateOfOccurrence = keySelector.DateOfProcess,
                keySelector.Source,
                keySelector.Target,
                keySelector.IsTaxable,
                keySelector.TaxType
            })
            .Select(g => new CashFlowModel(
                g.Key.DateOfOccurrence,
                g.Sum(item => item.Amount),
                g.Key.Source,
                g.Key.Target,
                g.Key.IsTaxable,
                g.Key.TaxType));
    }

    private static IEnumerable<ICashFlowDefinition> CreateCashFlows(
        this OrdinaryRetirementAction cashFlowAction, MultiPeriodCalculatorPerson person)
    {
        DateTime retirementDate = person.DateOfBirth.GetRetirementDate(person.Gender);

        yield return new StaticGenericCashFlowDefinition
        {
            Header = new CashFlowHeader
            {
                Id = "AhvPension",
                Name = "Yearly AHV Pension"
            },
            DateOfProcess = retirementDate,
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = retirementDate.Year,
                NumberOfPeriods = cashFlowAction.NumberOfPeriods,
            },
            Flow = new FlowPair(AccountType.Exogenous, AccountType.Income),
            InitialAmount = decimal.Zero,
            NetGrowthRate = decimal.Zero,
            RecurringInvestment = new RecurringInvestment
            {
                Amount = cashFlowAction.AhvPensionAmount,
                Frequency = FrequencyType.Yearly,
            },
            IsTaxable = true,
            TaxType = TaxType.Income
        };

        yield return new StaticGenericCashFlowDefinition
        {
            Header = new CashFlowHeader
            {
                Id = "RetirementPension",
                Name = "Yearly Retirement Pension"
            },
            DateOfProcess = retirementDate,
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = retirementDate.Year,
                NumberOfPeriods = cashFlowAction.NumberOfPeriods,
            },
            Flow = new FlowPair(AccountType.Exogenous, AccountType.Income),
            InitialAmount = decimal.Zero,
            NetGrowthRate = decimal.Zero,
            RecurringInvestment = new RecurringInvestment
            {
                Amount = cashFlowAction.RetirementPension,
                Frequency = FrequencyType.Yearly,
            },
            IsTaxable = true,
            TaxType = TaxType.Income
        };

        yield return new StaticGenericCashFlowDefinition
        {
            Header = new CashFlowHeader
            {
                Id = "CapitalConsumption",
                Name = "Yearly Capital Consumption"
            },
            DateOfProcess = retirementDate,
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = retirementDate.Year,
                NumberOfPeriods = cashFlowAction.NumberOfPeriods,
            },
            Flow = new FlowPair(AccountType.Wealth, AccountType.Exogenous),
            InitialAmount = decimal.Zero,
            NetGrowthRate = decimal.Zero,
            RecurringInvestment = new RecurringInvestment
            {
                Amount = cashFlowAction.CapitalConsumptionAmountPerYear,
                Frequency = FrequencyType.Yearly,
            },
            IsTaxable = false,
            TaxType = TaxType.Undefined
        };

        yield return new DynamicTransferAccountAction
        {
            Header = new CashFlowHeader { Id = "Clear3aAccount", Name = "Clear 3a Account" },
            DateOfProcess = retirementDate,
            TransferRatio = decimal.One,
            Flow = new FlowPair(AccountType.ThirdPillar, AccountType.Wealth),
            IsTaxable = true,
            TaxType = TaxType.CapitalBenefits
        };

        yield return new DynamicTransferAccountAction
        {
            Header = new CashFlowHeader { Id = "ClearOccupationalPensionAccount", Name = "Clear Pension Account" },
            DateOfProcess = retirementDate,
            TransferRatio = decimal.One,
            Flow = new FlowPair(AccountType.OccupationalPension, AccountType.Exogenous),
            IsTaxable = false,
            TaxType = TaxType.Undefined
        };
    }
}
