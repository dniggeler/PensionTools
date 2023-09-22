using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Bvg;
using Application.Extensions;
using Application.Municipality;
using Application.Tax.Proprietary.Abstractions;
using Application.Tax.Proprietary.Abstractions.Models;
using Calculators.CashFlow.Accounts;
using Calculators.CashFlow.Models;
using Domain.Contracts;
using Domain.Enums;
using Domain.Models.MultiPeriod;
using Domain.Models.MultiPeriod.Actions;
using Domain.Models.MultiPeriod.Definitions;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.BvgCalculator;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.Tax;

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
        int startingYear,
        int minimumNumberOfPeriods,
        MultiPeriodCalculatorPerson person,
        CashFlowDefinitionHolder cashFlowDefinitionHolder,
        MultiPeriodOptions options)
    {
        DateTime dateOfStart = new DateTime(startingYear, 1, 1);

        ExogenousAccount exogenousAccount = new() { Id = Guid.NewGuid(), Name = "Exogenous Account", };

        IncomeAccount incomeAccount = new() { Id = Guid.NewGuid(), Name = "Income Account", };

        WealthAccount wealthAccount = new() { Id = Guid.NewGuid(), Name = "Wealth Account", };

        InvestmentAccount investmentAccount = SetupInvestmentAccount(cashFlowDefinitionHolder);

        OccupationalPensionAccount occupationalPensionAccount = new()
        {
            Id = Guid.NewGuid(), Name = "Occupational Pension Account",
        };

        ThirdPillarAccount thirdPillarAccount = new()
        {
            Id = Guid.NewGuid(),
            Name = "Third Pillar Account",
        };

        TaxAccount taxAccount = new() { Id = Guid.NewGuid(), Name = "Tax Account", };

        IEnumerable<ICashFlowDefinition> definitionFromInvestments = cashFlowDefinitionHolder.InvestmentDefinitions
            .SelectMany(investment => investment.CreateGenericDefinition());

        IEnumerable<ICashFlowDefinition> definitionFromComposites = cashFlowDefinitionHolder.Composites
            .SelectMany(composite => composite.CreateGenericDefinition(person, dateOfStart));

        IEnumerable<CashFlowModel> staticCashFlowsFromComposites = definitionFromComposites
            .Concat(definitionFromInvestments)
            .OfType<StaticGenericCashFlowDefinition>()
            .SelectMany(d => d.GenerateCashFlow())
            .AggregateCashFlows();

        IEnumerable<CashFlowModel> staticCashFlowsFromGenerics = cashFlowDefinitionHolder.StaticGenericCashFlowDefinitions
            .SelectMany(d => d.GenerateCashFlow())
            .AggregateCashFlows();

        IEnumerable<CashFlowModel> staticCashFlows = staticCashFlowsFromGenerics
            .Concat(staticCashFlowsFromComposites)
            .ToList();

        var combinedList = definitionFromComposites
            .Concat(cashFlowDefinitionHolder.CashFlowActions).ToList();

        int finalYear = combinedList.Count > 0
            ? combinedList.Max(d => d.DateOfProcess.Year)
            : startingYear + minimumNumberOfPeriods;

        List<SinglePeriodCalculationResult> singlePeriodCalculationResults =
            Enumerable.Empty<SinglePeriodCalculationResult>().ToList();

        Dictionary<AccountType, ICashFlowAccount> currentAccounts = new Dictionary<AccountType, ICashFlowAccount>
        {
            { AccountType.Exogenous, exogenousAccount },
            { AccountType.Income, incomeAccount },
            { AccountType.Wealth, wealthAccount },
            { AccountType.Investment, investmentAccount },
            { AccountType.OccupationalPension, occupationalPensionAccount },
            { AccountType.ThirdPillar, thirdPillarAccount },
            { AccountType.Tax, taxAccount }
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

                List<CashFlowModel> currentDateStaticCashFlows = staticCashFlows
                    .Where(item => item.DateOfProcess == currentDate)
                    .ToList();

                List<ChangeResidenceAction> currentDateChangeResidenceActions = cashFlowDefinitionHolder
                    .ChangeResidenceActions
                    .Where(item => item.DateOfProcess == currentDateAsDateTime)
                    .ToList();

                List<CashFlowModel> currentDateDynamicCashFlows = definitionFromComposites
                    .Concat(cashFlowDefinitionHolder.CashFlowActions)
                    .OfType<IDynamicCashFlowDefinition>()
                    .Where(item => item.DateOfProcess == currentDateAsDateTime)
                    .SelectMany(item => item.CreateGenericDefinition(currentAccounts))
                    .SelectMany(item => item.GenerateCashFlow())
                    .ToList();

                // 1. change residence
                foreach (var changeAction in currentDateChangeResidenceActions.OfType<ChangeResidenceAction>())
                {
                    currentPerson = currentPerson with
                    {
                        MunicipalityId = changeAction.DestinationMunicipalityId,
                        Canton = changeAction.DestinationCanton,
                    };

                    currentAccounts = ProcessResidenceChangeAction(currentAccounts, changeAction);
                }

                // 2. process simple cash-flow: move amount from source to target account
                foreach (CashFlowModel cashFlow in currentDateStaticCashFlows.Concat(currentDateDynamicCashFlows))
                {
                    currentAccounts = await ProcessSimpleCashFlowAsync(currentAccounts, cashFlow, person);
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

        var accountTransactionResult = new AccountTransactionResult
        {
            Id = exogenousAccount.Id,
            Name = exogenousAccount.Name,
            Transactions = exogenousAccount.Transactions,
        };

        var incomeTransactionResult = new AccountTransactionResult
        {
            Id = incomeAccount.Id,
            Name = incomeAccount.Name,
            Transactions = incomeAccount.Transactions,
        };

        var wealthTransactionResult = new AccountTransactionResult
        {
            Id = wealthAccount.Id,
            Name = wealthAccount.Name,
            Transactions = wealthAccount.Transactions,
        };

        var investmentTransactionResult = new AccountTransactionResult
        {
            Id = investmentAccount.Id,
            Name = investmentAccount.Name,
            Transactions = investmentAccount.Transactions,
        };

        var occupationalTransactionResult = new AccountTransactionResult
        {
            Id = occupationalPensionAccount.Id,
            Name = occupationalPensionAccount.Name,
            Transactions = occupationalPensionAccount.Transactions,
        };

        var thirdPillarTransactionResult = new AccountTransactionResult
        {
            Id = thirdPillarAccount.Id,
            Name = thirdPillarAccount.Name,
            Transactions = thirdPillarAccount.Transactions,
        };

        var taxTransactionResult = new AccountTransactionResult
        {
            Id = taxAccount.Id,
            Name = taxAccount.Name,
            Transactions = taxAccount.Transactions,
        };

        return new MultiPeriodCalculationResult
        {
            StartingYear = startingYear,
            NumberOfPeriods = finalYear - startingYear + 1,
            Accounts = singlePeriodCalculationResults,
            Transactions = new AccountTransactionResultHolder
            {
                ExogenousAccount = accountTransactionResult,
                IncomeAccount = incomeTransactionResult,
                WealthAccount = wealthTransactionResult,
                InvestmentAccount = investmentTransactionResult,
                OccupationalPensionAccount = occupationalTransactionResult,
                ThirdPillarAccount = thirdPillarTransactionResult,
                TaxAccount = taxTransactionResult
            }
        };
    }

    private static InvestmentAccount SetupInvestmentAccount(CashFlowDefinitionHolder cashFlowDefinitionHolder)
    {
        InvestmentPortfolioDefinition firstInvestmentAccount = cashFlowDefinitionHolder.InvestmentDefinitions.FirstOrDefault();

        if (firstInvestmentAccount == null)
        {
            return new InvestmentAccount
            {
                Id = Guid.NewGuid(),
                Name = "Investment Account",
                NetGrowthRate = 0.0M,
                NetIncomeYield = 0.0M,
            };
        }

        return new()
        {
            Id = Guid.NewGuid(),
            Name = firstInvestmentAccount.Header.Name,
            NetGrowthRate = firstInvestmentAccount.NetCapitalGrowthRate,
            NetIncomeYield = firstInvestmentAccount.NetIncomeRate,
        };
    }

    public Task<Either<string, MultiPeriodCalculationResult>> CalculateWithSetupsAsync(
        int startingYear,
        int minimumNumberOfPeriods,
        MultiPeriodCalculatorPerson person,
        CashFlowDefinitionHolder cashFlowDefinitionHolder,
        MultiPeriodOptions options)
    {
        ICompositeCashFlowDefinition accountSetupDefinition = new SetupAccountDefinition
            {
                InitialOccupationalPensionAssets = person.CapitalBenefits.PensionPlan + person.CapitalBenefits.Pillar3a,
                InitialWealth = person.Wealth
            };

        CashFlowDefinitionHolder extendedDefinitionHolder = cashFlowDefinitionHolder with
        {
            Composites = cashFlowDefinitionHolder.Composites
                .Concat(new[] { accountSetupDefinition })
                .ToList()
        };

        return CalculateAsync(startingYear, minimumNumberOfPeriods, person, extendedDefinitionHolder, options);
    }

    private async Task<Dictionary<AccountType, ICashFlowAccount>> ProcessSimpleCashFlowAsync(
        Dictionary<AccountType, ICashFlowAccount> currentAccounts, CashFlowModel cashFlow, MultiPeriodCalculatorPerson person)
    {
        ICashFlowAccount creditAccount = currentAccounts[cashFlow.Target];
        ICashFlowAccount debitAccount = currentAccounts[cashFlow.Source];

        ExecuteTransaction(debitAccount, creditAccount, "Simple cash-flow", cashFlow.DateOfProcess.ToDateTime(TimeOnly.MinValue), cashFlow.Amount);

        if (cashFlow.IsTaxable)
        {
            // Tax reduces wealth as transaction is taxable.
            if (cashFlow.TaxType == TaxType.CapitalBenefits)
            {
                var taxPaymentAmount = await CalculateCapitalBenefitsTaxAsync(cashFlow.DateOfProcess.Year, person, cashFlow.Amount);

                ICashFlowAccount wealthAccount = currentAccounts[AccountType.Wealth];
                ICashFlowAccount taxAccount = currentAccounts[AccountType.Tax];

                ExecuteTransaction(wealthAccount, taxAccount, "Tax payment", cashFlow.DateOfProcess.ToDateTime(TimeOnly.MinValue), taxPaymentAmount);
            }
        }

        return currentAccounts;
    }

    private Dictionary<AccountType, ICashFlowAccount> ProcessResidenceChangeAction(
        Dictionary<AccountType, ICashFlowAccount> currentAccounts, ChangeResidenceAction action)
    {
        ICashFlowAccount wealthAccount = currentAccounts[AccountType.Wealth];
        ICashFlowAccount exogenousAccount = currentAccounts[AccountType.Exogenous];

        ExecuteTransaction(wealthAccount, exogenousAccount, "Residence change costs", action.DateOfProcess, action.ChangeCost);

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
        ICashFlowAccount investmentAccount = currentAccounts[AccountType.Investment];
        ICashFlowAccount exogenousAccount = currentAccounts[AccountType.Exogenous];
        ICashFlowAccount occupationalPensionAccount = currentAccounts[AccountType.OccupationalPension];
        ICashFlowAccount thirdPillarAccount = currentAccounts[AccountType.ThirdPillar];
        ICashFlowAccount taxAccount = currentAccounts[AccountType.Tax];

        DateTime finalDateAsDateTime = finalDate.ToDateTime(TimeOnly.MinValue);

        // compound wealth and capital benefits accounts
        // todo: instead of assuming account balance is compounded a full year a time-weighted calculation should be used (TWR).
        // This can be achieved by the ordered list of transaction on the accounts.
        decimal wealthCompoundedReturn = wealthAccount.Balance * options.WealthNetGrowthRate;
        ExecuteTransaction(exogenousAccount, wealthAccount, "Compound Return Wealth", finalDateAsDateTime, wealthCompoundedReturn);

        InvestmentTransactions(investmentAccount);

        decimal occupationalPensionCompoundedReturn = occupationalPensionAccount.Balance * options.CapitalBenefitsNetGrowthRate;
        ExecuteTransaction(exogenousAccount, occupationalPensionAccount, "Compound Return Occupational Pension", finalDateAsDateTime, occupationalPensionCompoundedReturn);

        decimal thirdPillarCompoundedReturn = thirdPillarAccount.Balance * options.CapitalBenefitsNetGrowthRate;
        ExecuteTransaction(exogenousAccount, thirdPillarAccount, "Compound Return Third Pillar", finalDateAsDateTime, thirdPillarCompoundedReturn);

        // savings quota: take share from current income account and move it to wealth
        decimal newSavings = incomeAccount.Balance * SavingsQuota(finalDate, options, person);

        // savings are subject to wealth tax but are not deducted from the income/salary account to keep taxable salary amount clean
        ExecuteTransaction(exogenousAccount, wealthAccount, "Savings Quota", finalDateAsDateTime, newSavings);

        // take each account amount, calculate tax, and deduct it from wealth
        var totalTaxAmount =
            await CalculateIncomeAndWealthTaxAsync(finalDate.Year, person, incomeAccount.Balance, wealthAccount.Balance + investmentAccount.Balance);

        ExecuteTransaction(wealthAccount, taxAccount, "Yearly Income and Wealth Tax", finalDateAsDateTime, totalTaxAmount);

        // Clear income account as it begin at 0 in the new year
        ExecuteTransaction(incomeAccount, exogenousAccount, "Clear income account", finalDateAsDateTime, incomeAccount.Balance);

        return currentAccounts;

        void InvestmentTransactions(ICashFlowAccount cashFlowAccount)
        {
            var account = cashFlowAccount as InvestmentAccount;
            
            if (account == null)
            {
                throw new ArgumentException($"Account {cashFlowAccount.Name} is not a investment account", nameof(cashFlowAccount));
            }

            decimal investmentCapitalGain = account.Balance * account.NetGrowthRate;
            decimal investmentIncome = account.Balance * account.NetIncomeYield;

            ExecuteTransaction(exogenousAccount, investmentAccount, "Capital Gains", finalDateAsDateTime, investmentCapitalGain);
            ExecuteTransaction(exogenousAccount, investmentAccount, "Income", finalDateAsDateTime, investmentIncome);

            // income is taxable
            ExecuteTransaction(exogenousAccount, incomeAccount, "Investement Income", finalDateAsDateTime, investmentCapitalGain);
        }
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
            TaxableWealth = Math.Max(0, wealth),
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

    private decimal SavingsQuota(DateOnly dateOfValidity, MultiPeriodOptions options, MultiPeriodCalculatorPerson person)
    {
        DateOnly retirementDate = DateOnly.FromDateTime(person.DateOfBirth.GetRetirementDate(person.Gender));

        if (dateOfValidity < retirementDate)
        {
            return options.SavingsQuota;
        }

        return decimal.Zero;
    }
}
