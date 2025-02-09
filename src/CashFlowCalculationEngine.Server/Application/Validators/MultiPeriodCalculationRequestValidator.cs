using CashFlowCalculationEngine.Server.Domain.Calculator;
using FluentValidation;

namespace CashFlowCalculationEngine.Server.Application.Validators;

public class MultiPeriodCalculationRequestValidator : AbstractValidator<MultiPeriodCalculationRequest>
{
    public MultiPeriodCalculationRequestValidator()
    {
        RuleFor(x =>x.AccountHolder)
            .SetValidator(new AccountInputValidator());

        RuleFor(x => x.CashFlowHolder)
            .SetValidator(new CashFlowInputValidator());

        RuleFor(x => x)
            .Must(Exist)
            .WithMessage("One or multiple cash-flows reference a non-existing account.");
    }

    private bool Exist(MultiPeriodCalculationRequest request)
    {
        HashSet<Guid> accountIds = [..ValidatorHelpers.AccountIdList(request.AccountHolder)];

        HashSet<Guid> cashFlowAccountIds =
            request.CashFlowHolder.FixedAmountCashFlows
                .SelectMany(cf => new[] { cf.SourceAccountId, cf.TargetAccountId })
                .Concat(request.CashFlowHolder.TransferRatioCashFlows
                    .SelectMany(cf => new[] { cf.SourceAccountId, cf.TargetAccountId }))
                .ToHashSet();

        // check if all cash-flow account ids are in the account ids
        return cashFlowAccountIds.All(accountIds.Contains);
    }
}
