using CashFlowCalculationEngine.Server.Domain.CashFlows;
using FluentValidation;

namespace CashFlowCalculationEngine.Server.Application.Validators;

public class CashFlowInputValidator : AbstractValidator<CashFlowInput>
{
    public CashFlowInputValidator()
    {
        RuleForEach(x => x.FixedAmountCashFlows)
            .ChildRules(a => a.RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(decimal.Zero))
            .WithMessage($"{nameof(FixedAmountCashFlow.Amount)} must be greater than zero.");

        RuleForEach(x => x.TransferRatioCashFlows)
            .Must(x => x.TransferFactor is >= 0 and <= 1)
            .WithMessage($"{nameof(TransferRatioCashFlow.TransferFactor)} must be between 0 and 1");

        RuleForEach(x => x.BalanceGrowthCashFlows)
            .Must(x => x.NetReturn is >= 0 and <= 1)
            .WithMessage($"{nameof(BalanceGrowthCashFlow.NetReturn)} must be between 0 and 1");

        RuleForEach(x => x.TransferRatioCashFlows)
            .Must(x => (x.Description?.Length ?? 0) <= 50)
            .WithMessage($"{nameof(SingleCashFlow.Description)} must be less than or equal to 50 characters");

        RuleForEach(x => x.TransferRatioCashFlows)
            .Must(x => x.TransferFactor is > decimal.Zero and <= decimal.One)
            .WithMessage("Transfer factor must be in range ]0, 1]");

        RuleForEach(x => x.FixedAmountCashFlows)
            .Must(x => (x.Description?.Length ?? 0) <= 50)
            .WithMessage($"{nameof(SingleCashFlow.Description)} must be less than or equal to 50 characters");
    }
}
