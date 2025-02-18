using Domain.Models.AccountInputs;
using FluentValidation;
using FluentValidation.Results;

namespace CashFlowCalculationEngine.Server.Application.Validators;

public class AccountInputValidator : AbstractValidator<AccountInput>
{
    const int MaxAccounts = 100;

    private List<Guid> accountIdList = [];

    public AccountInputValidator()
    {
        RuleFor(x => x)
            .Must(UniqueIds)
                .WithMessage("Referenced accounts are not unique")
            .Must(_ => accountIdList.Count <= MaxAccounts)
                .WithMessage("Max allowed number of accounts exceeded");
    }

    protected override bool PreValidate(ValidationContext<AccountInput> context, ValidationResult result)
    {
        AccountInput? model = context.InstanceToValidate;

        accountIdList = ValidatorHelpers.AccountIdList(model);

        return true;
    }

    private bool UniqueIds(AccountInput accountInput)
    {
        // check if all ids are unique
        return accountIdList.Distinct().Count() == accountIdList.Count;
    }
}
