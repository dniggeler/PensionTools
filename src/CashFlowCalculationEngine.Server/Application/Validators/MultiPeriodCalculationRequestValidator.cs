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
            .Must(ExistCashFlowAccounts)
            .WithMessage("One or multiple cash-flows refer to non-existing accounts.");

        RuleFor(x => x)
            .Must(ExistTaxActionAccounts)
            .WithMessage("One or multiple tax actions refer to non-existing accounts.");
    }

    private bool ExistCashFlowAccounts(MultiPeriodCalculationRequest request)
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

    private bool ExistTaxActionAccounts(MultiPeriodCalculationRequest request)
    {
        HashSet<Guid> accountIds = [.. ValidatorHelpers.AccountIdList(request.AccountHolder)];

        HashSet<Guid> taxActionAccountIds = [
            request.TaxationActionHolder.TaxPaymentSourceAccountId,
            request.TaxationActionHolder.TaxPaymentTargetAccountId
        ];

        // check if all cash-flow account ids are in the account ids
        return taxActionAccountIds.All(accountIds.Contains);
    }
}
