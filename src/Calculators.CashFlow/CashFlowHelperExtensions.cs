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
            definition.TaxType,
            definition.OccurrenceType);

        decimal cashFlow = definition.RecurringInvestment.Amount;

        foreach (var year in range)
        {
            yield return new CashFlowModel(
                new DateOnly(year, 1, 1),
                cashFlow,
                definition.Flow.Source,
                definition.Flow.Target,
                definition.IsTaxable,
                definition.TaxType,
                definition.OccurrenceType);

            cashFlow *= decimal.One + definition.NetGrowthRate;
        }
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
            DateOfStart = accountDefinition.DateOfStart,
            InitialAmount = accountDefinition.InitialWealth,
            Flow = new FlowPair(AccountType.Exogenous, AccountType.Wealth),
            RecurringInvestment = new RecurringInvestment
            {
                Amount = 0,
                Frequency = FrequencyType.Yearly,
            },
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = accountDefinition.DateOfStart.Year,
                NumberOfPeriods = 0
            },
            IsTaxable = true,
            TaxType = TaxType.Wealth,
            OccurrenceType = OccurrenceType.BeginOfPeriod
        };

        yield return new GenericCashFlowDefinition
        {
            Header = new CashFlowHeader
            {
                Id = "setupOccupationalPensionAccount",
                Name = "Occupational Pension Account"
            },
            DateOfStart = accountDefinition.DateOfStart,
            InitialAmount = accountDefinition.InitialOccupationalPensionAssets,
            Flow = new FlowPair(AccountType.Exogenous, AccountType.OccupationalPension),
            RecurringInvestment = new RecurringInvestment
            {
                Amount = 0,
                Frequency = FrequencyType.Yearly,
            },
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = accountDefinition.DateOfStart.Year,
                NumberOfPeriods = 0
            },
            IsTaxable = false,
            TaxType = TaxType.CapitalBenefits,
            OccurrenceType = OccurrenceType.BeginOfPeriod
        };

        yield return new GenericCashFlowDefinition
        {
            Header = new CashFlowHeader
            {
                Id = "setupThirdPillarAccount",
                Name = "Third Pillar Account"
            },
            DateOfStart = accountDefinition.DateOfStart,
            InitialAmount = accountDefinition.InitialThirdPillarAssets,
            Flow = new FlowPair(AccountType.Exogenous, AccountType.ThirdPillar),
            RecurringInvestment = new RecurringInvestment
            {
                Amount = 0,
                Frequency = FrequencyType.Yearly,
            },
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = accountDefinition.DateOfStart.Year,
                NumberOfPeriods = 0
            },
            IsTaxable = false,
            TaxType = TaxType.CapitalBenefits,
            OccurrenceType = OccurrenceType.BeginOfPeriod
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
            DateOfStart = purchaseDefinition.DateOfStart,
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
            TaxType = TaxType.Undefined,
            OccurrenceType = OccurrenceType.BeginOfPeriod
        };
    }

    public static GenericCashFlowDefinition CreateGenericDefinition(this SalaryPaymentsDefinition salaryDefinition)
    {
        return new GenericCashFlowDefinition
        {
            Header = salaryDefinition.Header,
            DateOfStart = salaryDefinition.DateOfStart,
            InvestmentPeriod = new InvestmentPeriod
            {
                Year = salaryDefinition.DateOfStart.Year,
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
            OccurrenceType = OccurrenceType.BeginOfPeriod,
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
            DateOfStart = thirdPillarDefinition.DateOfStart,
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
            TaxType = TaxType.Undefined,
            OccurrenceType = OccurrenceType.BeginOfPeriod
        };
    }

    public static IEnumerable<GenericCashFlowDefinition> CreateGenericDefinition(
        this ICashFlowAction cashFlowAction, MultiPeriodCalculatorPerson person)
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
                keySelector.TaxType,
                keySelector.OccurrenceType
            })
            .Select(g => new CashFlowModel(
                g.Key.DateOfOccurrence,
                g.Sum(item => item.Amount),
                g.Key.Source,
                g.Key.Target,
                g.Key.IsTaxable,
                g.Key.TaxType,
                g.Key.OccurrenceType));
    }

    private static IEnumerable<GenericCashFlowDefinition> CreateCashFlows(
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
            DateOfStart = retirementDate,
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
            OccurrenceType = OccurrenceType.BeginOfPeriod,
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
            DateOfStart = retirementDate,
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
            OccurrenceType = OccurrenceType.BeginOfPeriod,
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
            DateOfStart = retirementDate,
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
            OccurrenceType = OccurrenceType.BeginOfPeriod,
            IsTaxable = false,
            TaxType = TaxType.Undefined
        };
    }
}
