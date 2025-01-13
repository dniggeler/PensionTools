using CashFlowCalculationEngine.Server.Domain.Graph.Accounts;
using FluentValidation;

namespace CashFlowCalculationEngine.Server.Application.Validators;

public class AccountInputValidator : AbstractValidator<AccountInput>
{
    public AccountInputValidator()
    {
        RuleFor(x => x)
            .Must(x => UniqueIds(x))
            .WithMessage("Referenced accounts are not unique");
    }

    private bool UniqueIds(AccountInput accountInput)
    {
        IEnumerable<Guid> idList =
            accountInput.ExogenousAccounts.Select(a => a.Id)
            .Concat(accountInput.IncomeAccounts.Select(a => a.Id))
            .Concat(accountInput.WealthAccounts.Select(a => a.Id))
            .Concat(accountInput.OccupationalPensionAccounts.Select(a => a.Id))
            .Concat(accountInput.ThirdPillarAccounts.Select(a => a.Id))
            .Concat(accountInput.InvestmentAccounts.Select(a => a.Id))
            .ToList();

        // check if all ids are unique
        return idList.Distinct().Count() == idList.Count();
    }
}
