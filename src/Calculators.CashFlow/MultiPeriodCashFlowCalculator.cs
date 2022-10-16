﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calculators.CashFlow.Accounts;
using Calculators.CashFlow.Models;
using LanguageExt;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Actions;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace Calculators.CashFlow;

public class MultiPeriodCashFlowCalculator : IMultiPeriodCashFlowCalculator
{
    private readonly IFullWealthAndIncomeTaxCalculator _fullTaxCalculator;
    private readonly IFullCapitalBenefitTaxCalculator _capitalBenefitCalculator;
    private readonly IMunicipalityConnector municipalityConnector;
    private readonly ILogger<MultiPeriodCashFlowCalculator> _logger;

    public MultiPeriodCashFlowCalculator(
        IFullWealthAndIncomeTaxCalculator fullTaxCalculator,
        IFullCapitalBenefitTaxCalculator capitalBenefitCalculator,
        IMunicipalityConnector municipalityConnector,
        ILogger<MultiPeriodCashFlowCalculator> logger)
    {
        _fullTaxCalculator = fullTaxCalculator;
        _capitalBenefitCalculator = capitalBenefitCalculator;
        this.municipalityConnector = municipalityConnector;
        _logger = logger;
    }


    /// <inheritdoc />
    public async Task<Either<string, MultiPeriodCalculationResult>> CalculateAsync(
        MultiPeriodCalculatorPerson person,
        CashFlowDefinitionHolder cashFlowDefinitionHolder,
        MultiPeriodOptions options)
    {
        ExogenousAccount exogenousAccount = new() { Id = Guid.NewGuid(), Name = "Exogenous Account", };

        IncomeAccount incomeAccount = new() { Id = Guid.NewGuid(), Name = "Income Account", };

        WealthAccount wealthAccount = new() { Id = Guid.NewGuid(), Name = "Wealth Account", };

        CapitalBenefitsAccount capitalBenefitsAccount = new()
        {
            Id = Guid.NewGuid(), Name = "Capital Benefits Account",
        };

        IEnumerable<CashFlowModel> cashFlows = cashFlowDefinitionHolder.GenericCashFlowDefinitions
            .SelectMany(d => d.GenerateCashFlow())
            .AggregateCashFlows()
            .ToList();

        int startingYear = cashFlows.Min(item => item.DateOfProcess.Year);
        int finalYear = cashFlows.Max(item => item.DateOfProcess.Year);

        List<SinglePeriodCalculationResult> singlePeriodCalculationResults =
            Enumerable.Empty<SinglePeriodCalculationResult>().ToList();

        Dictionary<AccountType, ICashFlowAccount> currentAccounts = new Dictionary<AccountType, ICashFlowAccount>
        {
            { AccountType.Exogenous, exogenousAccount },
            { AccountType.Income, incomeAccount },
            { AccountType.Wealth, wealthAccount },
            { AccountType.CapitalBenefits, capitalBenefitsAccount }
        };

        MultiPeriodCalculatorPerson currentPerson = person;
        for (int currentYear = startingYear; currentYear <= finalYear; currentYear++)
        {
            DateOnly startingDate = new DateOnly(currentYear, 1, 1);
            DateOnly finalDate = new DateOnly(currentYear, 1, 1).AddYears(1);

            // all days in the current year
            for (DateOnly currentDate = startingDate; currentDate < finalDate; currentDate = currentDate.AddDays(1))
            {
                var currentDateAsDateTime = currentDate.ToDateTime(TimeOnly.MinValue);

                List<CashFlowModel> currentDateCashFlows = cashFlows
                    .Where(item => item.DateOfProcess == currentDate)
                    .ToList();

                List<ClearAccountAction> currentDateClearAccountActions = cashFlowDefinitionHolder
                    .ClearAccountActions
                    .Where(item => item.DateOfClearing == currentDateAsDateTime)
                    .ToList();

                List<ChangeResidenceAction> currentDateChangeResidenceActions = cashFlowDefinitionHolder
                    .ChangeResidenceActions
                    .Where(item => item.DateOfChange == currentDateAsDateTime)
                    .ToList();

                // 1. process simple cash-flow: move amount from source to target account
                foreach (var cashFlow in currentDateCashFlows)
                {
                    currentAccounts = ProcessSimpleCashFlow(currentAccounts, cashFlow);
                }

                // 2. apply taxable clearing cash-flows
                foreach (var clearAction in currentDateClearAccountActions.Where(item => item.IsTaxable))
                {
                    currentAccounts = await ProcessClearingActionAsync(currentAccounts, clearAction, person);
                }

                // 3. change residence
                foreach (var changeAction in currentDateChangeResidenceActions)
                {
                    currentPerson = currentPerson with
                    {
                        MunicipalityId = changeAction.DestinationMunicipalityId,
                        Canton = changeAction.DestinationCanton,
                    };

                    currentAccounts = ProcessResidenceChangeAction(currentAccounts, changeAction);
                }
            }

            currentAccounts = await ProcessEndOfYearSettlementAsync(currentAccounts, currentPerson, finalDate, options);

            // collect calculation results
            int year = currentYear;
            currentAccounts
                .Select(pair => new SinglePeriodCalculationResult
                {
                    Year = year, Amount = pair.Value.Balance, AccountType = pair.Key
                })
                .Iter(item => singlePeriodCalculationResults.Add(item));
        }

        return new MultiPeriodCalculationResult
        {
            StartingYear = startingYear,
            NumberOfPeriods = finalYear - startingYear + 1,
            Accounts = singlePeriodCalculationResults,
            ExogenousAccount = exogenousAccount,
            IncomeAccount = incomeAccount,
            WealthAccount = wealthAccount,
            CapitalBenefitsAccount = capitalBenefitsAccount
        };
    }

    /// <inheritdoc />
    public Task<Either<string, MultiPeriodCalculationResult>> CalculateAsync(
        int startingYear,
        int numberOfPeriods,
        MultiPeriodCalculatorPerson person,
        CashFlowDefinitionHolder cashFlowDefinitionHolder,
        MultiPeriodOptions options)
    {
        IEnumerable<GenericCashFlowDefinition> accountSetupDefinitions = new SetupAccountDefinition
            {
                DateOfStart = new DateTime(startingYear, 1, 1),
                InitialCapitalBenefits = person.CapitalBenefits.PensionPlan + person.CapitalBenefits.Pillar3a,
                InitialWealth = person.Wealth
            }
            .CreateGenericDefinition();

        GenericCashFlowDefinition salaryCashFlowDefinition = new SalaryPaymentsDefinition
            {
                Header = new CashFlowHeader { Id = "my salary", Name = $"{person.Name} - Lohn", Ordinal = 0, },
                DateOfStart = new DateTime(startingYear, 1, 1),
                YearlyAmount = person.Income,
                NumberOfInvestments = numberOfPeriods,
                NetGrowthRate = options.SalaryNetGrowthRate,
            }
            .CreateGenericDefinition();

        var extendedDefinitionHolder = cashFlowDefinitionHolder with
        {
            GenericCashFlowDefinitions = new[] { salaryCashFlowDefinition }
                .Concat(accountSetupDefinitions)
                .Concat(cashFlowDefinitionHolder.GenericCashFlowDefinitions)
                .ToList()
        };

        return CalculateAsync(person, extendedDefinitionHolder, options);
    }

    private Dictionary<AccountType, ICashFlowAccount> ProcessSimpleCashFlow(
        Dictionary<AccountType, ICashFlowAccount> currentAccounts, CashFlowModel cashFlow)
    {
        ICashFlowAccount creditAccount = currentAccounts[cashFlow.Target];
        ICashFlowAccount debitAccount = currentAccounts[cashFlow.Source];

        ExecuteTransaction(debitAccount, creditAccount, "Simple cash-flow", cashFlow.DateOfProcess.ToDateTime(TimeOnly.MinValue), cashFlow.Amount);

        return currentAccounts;
    }

    private async Task<Dictionary<AccountType, ICashFlowAccount>> ProcessClearingActionAsync(
        Dictionary<AccountType, ICashFlowAccount> currentAccounts, ClearAccountAction clearingAction, MultiPeriodCalculatorPerson person)
    {
        decimal taxableAmount = currentAccounts[clearingAction.Flow.Source].Balance * clearingAction.ClearRatio;

        ICashFlowAccount creditAccount = currentAccounts[clearingAction.Flow.Target];
        ICashFlowAccount debitAccount = currentAccounts[clearingAction.Flow.Source];

        ExecuteTransaction(debitAccount, creditAccount, "Clear account", clearingAction.DateOfClearing, taxableAmount);

        // Tax reduces wealth as transaction is taxable.
        var taxPaymentAmount = await CalculateCapitalBenefitsTaxAsync(clearingAction.DateOfClearing.Year, person, taxableAmount);

        ICashFlowAccount wealthAccount = currentAccounts[AccountType.Wealth];
        ICashFlowAccount exogenousAccount = currentAccounts[AccountType.Exogenous];

        ExecuteTransaction(wealthAccount, exogenousAccount, "Tax payment", clearingAction.DateOfClearing, taxPaymentAmount);

        return currentAccounts;
    }

    private Dictionary<AccountType, ICashFlowAccount> ProcessResidenceChangeAction(
        Dictionary<AccountType, ICashFlowAccount> currentAccounts, ChangeResidenceAction action)
    {
        ICashFlowAccount wealthAccount = currentAccounts[AccountType.Wealth];
        ICashFlowAccount exogenousAccount = currentAccounts[AccountType.Exogenous];

        ExecuteTransaction(wealthAccount, exogenousAccount, "Residence change costs", action.DateOfChange, action.ChangeCost);

        return currentAccounts;
    }

    private async Task<Dictionary<AccountType, ICashFlowAccount>> ProcessEndOfYearSettlementAsync(
        Dictionary<AccountType, ICashFlowAccount> currentAccounts,
        MultiPeriodCalculatorPerson person,
        DateOnly finalDate,
        MultiPeriodOptions options)
    {
        ICashFlowAccount incomeAccount = currentAccounts[AccountType.Income];
        ICashFlowAccount wealthAccount = currentAccounts[AccountType.Wealth];
        ICashFlowAccount exogenousAccount = currentAccounts[AccountType.Exogenous];
        ICashFlowAccount capitalBenefitsAccount = currentAccounts[AccountType.CapitalBenefits];

        DateTime finalDateAsDateTime = finalDate.ToDateTime(TimeOnly.MinValue);

        // compound wealth and capital benefits accounts
        // todo: instead of assuming account balance is compounded a full year a time-weighted calculation should be used (TWR).
        // This can be achieved by the ordered list of transaction on the accounts.
        decimal wealthCompoundedReturn = wealthAccount.Balance * options.WealthNetGrowthRate;
        ExecuteTransaction(exogenousAccount, wealthAccount, "Compound Return Wealth", finalDateAsDateTime, wealthCompoundedReturn);

        decimal capitalBenefitsCompoundedReturn = capitalBenefitsAccount.Balance * options.CapitalBenefitsNetGrowthRate;
        ExecuteTransaction(exogenousAccount, capitalBenefitsAccount, "Compound Return Capital Benefits", finalDateAsDateTime, capitalBenefitsCompoundedReturn);
        
        // savings quota: take share from current income account and move it to wealth
        decimal newSavings = incomeAccount.Balance * options.SavingsQuota;

        // savings are subject to wealth tax but are not deducted fr
        ExecuteTransaction(exogenousAccount, wealthAccount, "Savings Quota", finalDateAsDateTime, newSavings);

        // take each account amount, calculate tax, and deduct it from wealth
        var totalTaxAmount =
            await CalculateIncomeAndWealthTaxAsync(finalDate.Year, person, incomeAccount.Balance, wealthAccount.Balance);

        ExecuteTransaction(wealthAccount, exogenousAccount, "Yearly Income and Wealth Tax", finalDateAsDateTime, totalTaxAmount);

        // Clear income account as it begin at 0 in the new year
        ExecuteTransaction(incomeAccount, exogenousAccount, "Clear income account", finalDateAsDateTime, incomeAccount.Balance);

        return currentAccounts;
    }

    private async Task<decimal> CalculateIncomeAndWealthTaxAsync(
        int currentYear, MultiPeriodCalculatorPerson calculatorPerson, decimal income, decimal wealth)
    {
        TaxPerson taxPerson = new()
        {
            Name = calculatorPerson.Name,
            CivilStatus = calculatorPerson.CivilStatus,
            NumberOfChildren = calculatorPerson.NumberOfChildren,
            ReligiousGroupType = calculatorPerson.ReligiousGroupType,
            PartnerReligiousGroupType = calculatorPerson.PartnerReligiousGroupType,
            TaxableWealth = wealth,
            TaxableFederalIncome = income,
            TaxableIncome = income
        };

        Either<string, MunicipalityModel> municipality =
            await municipalityConnector.GetAsync(calculatorPerson.MunicipalityId, currentYear);

        Either<string, FullTaxResult> result = await municipality
            .BindAsync(m => _fullTaxCalculator.CalculateAsync(currentYear, m, taxPerson, true));

        return result.Match(
            Right: r => r.TotalTaxAmount,
            Left: error =>
            {
                _logger.LogError(error);
                return decimal.Zero;
            });
    }

    private async Task<decimal> CalculateCapitalBenefitsTaxAsync(
        int currentYear, MultiPeriodCalculatorPerson person, decimal amount)
    {
        CapitalBenefitTaxPerson taxPerson = new()
        {
            Name = person.Name,
            CivilStatus = person.CivilStatus,
            NumberOfChildren = person.NumberOfChildren,
            ReligiousGroupType = person.ReligiousGroupType,
            PartnerReligiousGroupType = person.PartnerReligiousGroupType,
            TaxableCapitalBenefits = amount
        };

        Either<string, MunicipalityModel> municipality =
            await municipalityConnector.GetAsync(person.MunicipalityId, currentYear);

        Either<string, FullCapitalBenefitTaxResult> result = await municipality
            .BindAsync(m => _capitalBenefitCalculator.CalculateAsync(
                currentYear, m, taxPerson, true));

        return result.Match(
            Right: r => r.TotalTaxAmount,
            Left: error =>
            {
                _logger.LogError(error);
                return decimal.Zero;

            });
    }

    private static void ExecuteTransaction(
        ICashFlowAccount debitAccount, ICashFlowAccount creditAccount, string description, DateTime transactionDate, decimal amount)
    {
        if (amount == decimal.Zero)
        {
            return;
        }

        AccountTransaction trxCreditAccount =
            new($"{description}: inflow from {debitAccount.Name}", transactionDate, amount);

        creditAccount.Balance += amount;
        creditAccount.Transactions.Add(trxCreditAccount);


        AccountTransaction trxDebitAccount =
            new($"{description}: outflow to {creditAccount.Name}", transactionDate, -amount);

        debitAccount.Balance -= amount;
        debitAccount.Transactions.Add(trxDebitAccount);
    }
}
