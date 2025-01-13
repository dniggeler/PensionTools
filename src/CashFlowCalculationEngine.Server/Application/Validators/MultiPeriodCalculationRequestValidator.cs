using CashFlowCalculationEngine.Server.Domain.Calculator;
using FluentValidation;

namespace CashFlowCalculationEngine.Server.Application.Validators;

public class MultiPeriodCalculationRequestValidator : AbstractValidator<MultiPeriodCalculationRequest>
{
    public MultiPeriodCalculationRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => Exist(x))
            .WithMessage("One or multiple cash-flows reference a non-existing account.");
    }

    private bool Exist(MultiPeriodCalculationRequest request)
    {
        HashSet<Guid> accountIds =
            request.AccountHolder.ExogenousAccounts.Select(a => a.Id)
            .Concat(request.AccountHolder.IncomeAccounts.Select(a => a.Id))
            .Concat(request.AccountHolder.WealthAccounts.Select(a => a.Id))
            .Concat(request.AccountHolder.OccupationalPensionAccounts.Select(a => a.Id))
            .Concat(request.AccountHolder.ThirdPillarAccounts.Select(a => a.Id))
            .Concat(request.AccountHolder.InvestmentAccounts.Select(a => a.Id))
            .ToHashSet();

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
