using Application.Municipality;
using Application.Tax.Contracts;
using CashFlowCalculationEngine.Server.Domain;
using CashFlowCalculationEngine.Server.Domain.Calculator;
using CashFlowCalculationEngine.Server.Domain.CashFlows;
using CashFlowCalculationEngine.Server.Domain.Graph.Accounts;
using CashFlowCalculationEngine.Server.Domain.Location;
using CashFlowCalculationEngine.Server.Domain.Person;
using Domain.Enums;
using Domain.Models.Cashflows;
using Domain.Models.Cashflows.Accounts;

namespace CashFlowCalculationEngine.Server.Application.Calculators;

public class MultiPeriodCashFlowCalculator(
    IFullWealthAndIncomeTaxCalculator wealthAndIncomeTaxCalculator,
    IFullCapitalBenefitTaxCalculator capitalBenefitTaxCalculator,
    IMunicipalityConnector municipalityConnector,
    ILogger<MultiPeriodCashFlowCalculator> logger) : IMultiPeriodCashFlowCalculator
{
    public Task<MultiPeriodCalculationResponse> CalculateAsync(
        CalculationParameters calculationParameters,
        CalculationPerson person,
        Municipality municipality,
        AccountInput accountHolder,
        CashFlowInput cashFlowHolder,
        CancellationToken cancellationToken)
    {
        Dictionary<Guid, ExogenousAccount> exogenousAccounts = Build(accountHolder.ExogenousAccounts);
        Dictionary<Guid, WealthAccount> wealthAccounts = Build(accountHolder.WealthAccounts);
        Dictionary<Guid, IncomeAccount> incomeAccounts = Build(accountHolder.IncomeAccounts);
        Dictionary<Guid, ThirdPillarAccount> thirdPillarAccounts = Build(accountHolder.ThirdPillarAccounts);
        Dictionary<Guid, OccupationalPensionAccount> occupationalPensionAccounts = Build(accountHolder.OccupationalPensionAccounts);
        Dictionary<Guid, InvestmentAccount> investmentAccounts = Build(accountHolder.InvestmentAccounts);

        // create a dictionary of all accounts
        Dictionary<Guid, ICashFlowAccount> allAccounts = new Dictionary<Guid,ICashFlowAccount>();
        exogenousAccounts.Iter(a => allAccounts.Add(a.Key, a.Value));
        wealthAccounts.Iter(a => allAccounts.Add(a.Key, a.Value));
        incomeAccounts.Iter(a => allAccounts.Add(a.Key, a.Value));
        thirdPillarAccounts.Iter(a => allAccounts.Add(a.Key, a.Value));
        occupationalPensionAccounts.Iter(a => allAccounts.Add(a.Key, a.Value));
        investmentAccounts.Iter(a => allAccounts.Add(a.Key, a.Value));

        List<SingleCashFlow> allCashFlows =
            cashFlowHolder.FixedAmountCashFlows
            .OfType<SingleCashFlow>()
            .Concat(cashFlowHolder.TransferRatioCashFlows)
            .Concat(cashFlowHolder.BalanceGrowthCashFlows)
            .ToList();

        int startingYear = calculationParameters.StartDate.Year;
        int finalYear = calculationParameters.EndDate.Year;

        for (int currentYear = startingYear; currentYear <= finalYear; currentYear++)
        {
            DateOnly startingDate = new DateOnly(currentYear, 1, 1);
            DateOnly finalDate = new DateOnly(currentYear, 1, 1).AddYears(1);

            // all days in the current year
            for (DateOnly currentDate = startingDate; currentDate < finalDate; currentDate = currentDate.AddDays(1))
            {
                DateOnly date = currentDate;
                IEnumerable<SingleCashFlow> currentDateCashFlows = allCashFlows
                    .Where(item => item.DateOfProcess == date)
                    .OrderBy(c => c.Sequence);

                // 2. process simple cash-flow: move amount from source to target account
                foreach (var cashFlow in currentDateCashFlows)
                {
                    ProcessSimpleCashFlow(allAccounts, cashFlow, person);
                }
            }
        }

        var exogenousTransactionResult = exogenousAccounts
            .Select(a => new AccountTransactionResult
            {
                Id = a.Key, Name = a.Value.Name, Transactions = a.Value.Transactions,
            });

        var incomeTransactionResult = incomeAccounts
            .Select(a => new AccountTransactionResult
            {
                Id = a.Key,
                Name = a.Value.Name,
                Transactions = a.Value.Transactions,
            });

        var investmentTransactionResult = investmentAccounts
            .Select(a => new AccountTransactionResult
            {
                Id = a.Key,
                Name = a.Value.Name,
                Transactions = a.Value.Transactions,
            });

        var wealthTransactionResult = wealthAccounts
            .Select(a => new AccountTransactionResult
            {
                Id = a.Key,
                Name = a.Value.Name,
                Transactions = a.Value.Transactions,
            });

        var thirdPillarTransactionResult = thirdPillarAccounts
            .Select(a => new AccountTransactionResult
            {
                Id = a.Key,
                Name = a.Value.Name,
                Transactions = a.Value.Transactions,
            });

        var occupationalTransactionResult = occupationalPensionAccounts
            .Select(a => new AccountTransactionResult
            {
                Id = a.Key,
                Name = a.Value.Name,
                Transactions = a.Value.Transactions,
            });


        MultiPeriodCalculationResponse response = new MultiPeriodCalculationResponse
        {
            CalculationId = Guid.NewGuid(),
            Transactions = new AccountTransactionResponse
            {
                ExogenousAccounts = exogenousTransactionResult,
                IncomeAccounts = incomeTransactionResult,
                WealthAccounts = wealthTransactionResult,
                InvestmentAccounts = investmentTransactionResult,
                OccupationalPensionAccounts = occupationalTransactionResult,
                ThirdPillarAccounts = thirdPillarTransactionResult,
            }
        };

        return Task.FromResult(response);
    }

    private Dictionary<Guid, ICashFlowAccount> ProcessSimpleCashFlow(
        Dictionary<Guid, ICashFlowAccount> currentAccounts, SingleCashFlow cashFlow, CalculationPerson person)
    {
        ICashFlowAccount creditAccount = currentAccounts[cashFlow.TargetAccountId];
        ICashFlowAccount debitAccount = currentAccounts[cashFlow.SourceAccountId];

        switch (cashFlow)
        {
            case TransferRatioCashFlow f:
                ExecuteTransferRatioCashFlow(
                    debitAccount, creditAccount, f.Description, cashFlow.DateOfProcess.ToDateTime(TimeOnly.MinValue), f.TransferFactor);
                break;
            case FixedAmountCashFlow f:
                ExecuteFixedCashFlow(
                    debitAccount, creditAccount, f.Description, cashFlow.DateOfProcess.ToDateTime(TimeOnly.MinValue), f.Amount);
                break;
            case BalanceGrowthCashFlow f:
                ExecuteBalanceGrowthCashFlow(
                    debitAccount, creditAccount, f.Description, cashFlow.DateOfProcess.ToDateTime(TimeOnly.MinValue), f.NetReturn);
                break;
        }

        return currentAccounts;
    }

    private static void ExecuteFixedCashFlow(
        ICashFlowAccount debitAccount, ICashFlowAccount creditAccount, string? description, DateTime transactionDate, decimal amount)
    {
        AccountTransaction trxCreditAccount =
            new($"{description}: inflow from {debitAccount.Name}", transactionDate, amount, FlowType.InFlow);

        creditAccount.Balance += amount;
        creditAccount.Transactions.Add(trxCreditAccount);


        AccountTransaction trxDebitAccount =
            new($"{description}: outflow to {creditAccount.Name}", transactionDate, -amount, FlowType.OutFlow);

        debitAccount.Balance -= amount;
        debitAccount.Transactions.Add(trxDebitAccount);
    }

    private static void ExecuteTransferRatioCashFlow(
        ICashFlowAccount debitAccount, ICashFlowAccount creditAccount, string? description, DateTime transactionDate, decimal ratio)
    {
        decimal amount = debitAccount.Balance * ratio;

        AccountTransaction trxCreditAccount =
            new($"{description}: inflow from {debitAccount.Name}", transactionDate, amount, FlowType.InFlow);

        creditAccount.Balance += amount;
        creditAccount.Transactions.Add(trxCreditAccount);


        AccountTransaction trxDebitAccount =
            new($"{description}: outflow to {creditAccount.Name}", transactionDate, -amount, FlowType.OutFlow);

        debitAccount.Balance -= amount;
        debitAccount.Transactions.Add(trxDebitAccount);
    }

    private static void ExecuteBalanceGrowthCashFlow(
        ICashFlowAccount debitAccount, ICashFlowAccount creditAccount, string? description, DateTime transactionDate, decimal netReturnDecimal)
    {
        decimal amount = creditAccount.Balance * netReturnDecimal;

        AccountTransaction trxCreditAccount =
            new($"{description}: inflow from {debitAccount.Name}", transactionDate, amount, FlowType.InFlow);

        creditAccount.Balance += amount;
        creditAccount.Transactions.Add(trxCreditAccount);


        AccountTransaction trxDebitAccount =
            new($"{description}: outflow to {creditAccount.Name}", transactionDate, -amount, FlowType.OutFlow);

        debitAccount.Balance -= amount;
        debitAccount.Transactions.Add(trxDebitAccount);
    }

    private Dictionary<Guid, ExogenousAccount> Build(IEnumerable<ExogenousAccountInput> accounts)
    {
        return accounts
            .Select(a => new ExogenousAccount
            {
                Id = a.Id,
                Name = a.Description ?? string.Empty,
            })
            .ToDictionary(keySelector: (a) => a.Id);
    }

    private Dictionary<Guid, WealthAccount> Build(IEnumerable<WealthAccountInput> accounts)
    {
        return accounts
            .Select(a => new WealthAccount
            {
                Id = a.Id,
                Name = a.Description ?? string.Empty,
            })
            .ToDictionary(keySelector: (a) => a.Id);
    }

    private Dictionary<Guid, IncomeAccount> Build(IEnumerable<IncomeAccountInput> accounts)
    {
        return accounts
            .Select(a => new IncomeAccount
            {
                Id = a.Id,
                Name = a.Description ?? string.Empty,
            })
            .ToDictionary(keySelector: (a) => a.Id);
    }

    private Dictionary<Guid, ThirdPillarAccount> Build(IEnumerable<ThirdPillarAccountInput> accounts)
    {
        return accounts
            .Select(a => new ThirdPillarAccount
            {
                Id = a.Id,
                Name = a.Description ?? string.Empty,
            })
            .ToDictionary(keySelector: (a) => a.Id);
    }

    private Dictionary<Guid, OccupationalPensionAccount> Build(IEnumerable<OccupationalPensionAccountInput> accounts)
    {
        return accounts
            .Select(a => new OccupationalPensionAccount
            {
                Id = a.Id,
                Name = a.Description ?? string.Empty,
            })
            .ToDictionary(keySelector: (a) => a.Id);
    }

    private Dictionary<Guid, InvestmentAccount> Build(IEnumerable<InvestmentAccountInput> accounts)
    {
        return accounts
            .Select(a => new InvestmentAccount
            {
                Id = a.Id,
                Name = a.Description ?? string.Empty,
            })
            .ToDictionary(keySelector: (a) => a.Id);
    }
}
