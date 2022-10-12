using System;
using System.Collections.Generic;
using System.Linq;
using Calculators.CashFlow.Models;
using LanguageExt;
using Microsoft.Extensions.Options;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
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
                Id = "setupCapitalBenefitsAccount",
                Name = "Capital Benefits Account"
            },
            DateOfStart = accountDefinition.DateOfStart,
            InitialAmount = accountDefinition.InitialCapitalBenefits,
            Flow = new FlowPair(AccountType.Exogenous, AccountType.CapitalBenefits),
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
            TaxType = TaxType.Capital,
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
            Flow = new FlowPair(AccountType.Income, AccountType.CapitalBenefits),
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
            Flow = new FlowPair(AccountType.Income, AccountType.CapitalBenefits),
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
}
