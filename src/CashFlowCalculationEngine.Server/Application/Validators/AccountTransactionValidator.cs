using CashFlowCalculationEngine.Server.Domain;
using FluentValidation;

namespace CashFlowCalculationEngine.Server.Application.Validators;

public class AccountTransactionValidator : AbstractValidator<AccountTransactionResponse?>
{
    public AccountTransactionValidator()
    {
        RuleFor(x => x)
            .Must(Exist)
            .WithMessage("One or multiple cash-flows reference a non-existing account.");
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
