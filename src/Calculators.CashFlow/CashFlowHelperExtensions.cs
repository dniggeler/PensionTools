using System;
using System.Collections.Generic;
using System.Linq;
using Calculators.CashFlow.Models;
using PensionCoach.Tools.BvgCalculator;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Actions;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;
using PensionCoach.Tools.CommonTypes.Tax;

namespace Calculators.CashFlow;

public static class CashFlowHelperExtensions
{
    public static IEnumerable<CashFlowModel> GenerateCashFlow(this ICashFlowDefinition definition)
    {
        return definition switch
        {
            GenericCashFlowDefinition d => d.GenerateCashFlow(),
            TransferAccountAction a => a.GenerateCashFlow(),
            _ => Array.Empty<CashFlowModel>()
        };
    }

    public static IEnumerable<CashFlowModel> GenerateCashFlow(this GenericCashFlowDefinition definition)
    {
        var range = Enumerable.Range(
            definition.InvestmentPeriod.Year,
            definition.InvestmentPeriod.NumberOfPeriods);

        yield return new CashFlowModel(
            new DateOnly(definition.InvestmentPeriod.Year, 1, 1),
            definition.InitialAmount,
            AccountType.Exogenous,
            definition.Flow.Target,
            definition.IsTaxable,
            definition.TaxType);

        decimal cashFlow = definition.RecurringInvestment.Amount;

        foreach (var year in range)
        {
            yield return new CashFlowModel(
                new DateOnly(year, 1, 1),
                cashFlow,
                definition.Flow.Source,
                definition.Flow.Target,
                definition.IsTaxable,
                definition.TaxType);

            cashFlow *= decimal.One + definition.NetGrowthRate;
        }
    }

    public static IEnumerable<CashFlowModel> GenerateCashFlow(this TransferAccountAction action)
    {
        yield return new CashFlowModel(
            DateOnly.FromDateTime(action.DateOfProcess),
            68_000,
            action.Flow.Source,
            action.Flow.Target,
            action.IsTaxable,
            action.TaxType);
    }

    public static IEnumerable<GenericCashFlowDefinition> CreateGenericDefinition(this SetupAccountDefinition accountDefinition)
    {
        yield return new GenericCashFlowDefinition
        {
            Header = new CashFlowHeader
            {
                Id = "setupWealthAccount",
                Name = "Wealth Account"
            },
            DateOfProcess = accountDefinition.DateOfProcess,
            InitialAmount = accountDefinition.InitialWealth,
            Flow = new FlowPair(AccountType.Exogenous, AccountType.Wealth),
            RecurringInvestment = new RecurringInvestment
            {
                Amount = 0,
                Frequency = FrequencyType.Yearly,
            },
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = accountDefinition.DateOfProcess.Year,
                NumberOfPeriods = 0
            },
            IsTaxable = true,
            TaxType = TaxType.Wealth
        };

        yield return new GenericCashFlowDefinition
        {
            Header = new CashFlowHeader
            {
                Id = "setupOccupationalPensionAccount",
                Name = "Occupational Pension Account"
            },
            DateOfProcess = accountDefinition.DateOfProcess,
            InitialAmount = accountDefinition.InitialOccupationalPensionAssets,
            Flow = new FlowPair(AccountType.Exogenous, AccountType.OccupationalPension),
            RecurringInvestment = new RecurringInvestment
            {
                Amount = 0,
                Frequency = FrequencyType.Yearly,
            },
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = accountDefinition.DateOfProcess.Year,
                NumberOfPeriods = 0
            },
            IsTaxable = false,
            TaxType = TaxType.CapitalBenefits
        };

        yield return new GenericCashFlowDefinition
        {
            Header = new CashFlowHeader
            {
                Id = "setupThirdPillarAccount",
                Name = "Third Pillar Account"
            },
            DateOfProcess = accountDefinition.DateOfProcess,
            InitialAmount = accountDefinition.InitialThirdPillarAssets,
            Flow = new FlowPair(AccountType.Exogenous, AccountType.ThirdPillar),
            RecurringInvestment = new RecurringInvestment
            {
                Amount = 0,
                Frequency = FrequencyType.Yearly,
            },
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = accountDefinition.DateOfProcess.Year,
                NumberOfPeriods = 0
            },
            IsTaxable = false,
            TaxType = TaxType.CapitalBenefits
        };
    }

    public static GenericCashFlowDefinition CreateGenericDefinition(this PensionPlanPaymentsDefinition purchaseDefinition)
    {
        return new GenericCashFlowDefinition
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
    }

    public static GenericCashFlowDefinition CreateGenericDefinition(this SalaryPaymentsDefinition salaryDefinition)
    {
        return new GenericCashFlowDefinition
        {
            Header = salaryDefinition.Header,
            DateOfProcess = salaryDefinition.DateOfProcess,
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = salaryDefinition.DateOfProcess.Year,
                NumberOfPeriods = salaryDefinition.NumberOfInvestments,
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
    }

    public static GenericCashFlowDefinition CreateGenericDefinition(this ThirdPillarPaymentsDefinition thirdPillarDefinition)
    {
        return new GenericCashFlowDefinition
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
    }

    public static IEnumerable<ICashFlowDefinition> CreateGenericDefinition(
        this ICashFlowDefinition cashFlowAction, MultiPeriodCalculatorPerson person)
    {
        return cashFlowAction switch
        {
            OrdinaryRetirementAction a => a.CreateCashFlows(person),
            _ => Array.Empty<GenericCashFlowDefinition>()
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

        yield return new GenericCashFlowDefinition
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

        yield return new GenericCashFlowDefinition
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

        yield return new GenericCashFlowDefinition
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

        yield return new TransferAccountAction
        {
            Header = new CashFlowHeader { Id = "Clear3aAccount", Name = "Clear 3a Account" },
            DateOfProcess = retirementDate,
            TransferRatio = decimal.One,
            Flow = new FlowPair(AccountType.ThirdPillar, AccountType.Wealth),
            IsTaxable = true,
            TaxType = TaxType.CapitalBenefits
        };

        yield return new TransferAccountAction
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
