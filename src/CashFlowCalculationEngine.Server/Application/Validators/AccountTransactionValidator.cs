using CashFlowCalculationEngine.Server.Domain;
using Domain.Enums;
using Domain.Models.Cashflows.Accounts;
using FluentValidation;

namespace CashFlowCalculationEngine.Server.Application.Validators;

public class AccountTransactionValidator : AbstractValidator<AccountTransactionResponse?>
{
    public AccountTransactionValidator()
    {
        RuleFor(x => x)
            .Must(Exist)
            .WithMessage("One or multiple cash-flows reference a non-existing account.");

        RuleFor(x => x)
            .Must(CheckBalance)
            .WithMessage("The total balance across all accounts must sum up to zero.");
    }

    private bool CheckBalance(AccountTransactionResponse? response)
    {
        List<AccountTransaction> allTransactions = [..ValidatorHelpers.AccountTransactionList(response)];

        // sum up all transactions of type in-flow
        decimal inflowBalance = allTransactions
            .Where(t => t.Flow == FlowType.InFlow)
            .Sum(t => t.Amount);

        decimal outflowBalance = allTransactions
            .Where(t => t.Flow == FlowType.OutFlow)
            .Sum(t => t.Amount);

        
        return Math.Abs(inflowBalance + outflowBalance) < 0.01M;
    }

    private bool Exist(AccountTransactionResponse? response)
    {
        if (response == null)
        {
            return true;
        }

        HashSet<Guid> accountInputIds = response
            .ThirdPillarAccounts.Select(a => a.Id)
            .Concat(response.ExogenousAccounts.Select(a => a.Id))
            .Concat(response.IncomeAccounts.Select(a => a.Id))
            .Concat(response.OccupationalPensionAccounts.Select(a => a.Id))
            .Concat(response.WealthAccounts.Select(a => a.Id))
            .Concat(response.InvestmentAccounts.Select(a => a.Id))
            .ToHashSet();
        HashSet<Guid> accountResponseIds = [.. ValidatorHelpers.AccountIdList(response)];

        // check if all cash-flow account ids are in the account ids
        return accountResponseIds.All(accountInputIds.Contains);
    }
}
