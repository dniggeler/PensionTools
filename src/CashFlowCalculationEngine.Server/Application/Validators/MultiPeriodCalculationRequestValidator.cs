using CashFlowCalculationEngine.Server.Domain;
using CashFlowCalculationEngine.Server.Domain.CashFlows;
using FluentValidation;

namespace CashFlowCalculationEngine.Server.Application.Validators;

public class MultiPeriodCalculationRequestValidator : AbstractValidator<MultiPeriodCalculationRequest>
{
    public MultiPeriodCalculationRequestValidator()
    {
        RuleForEach(x => x.CashFlowHolder.TransferRatioCashFlows)
            .Must(x => x.TransferFactor is >= 0 and <= 1)
            .WithMessage($"{nameof(TransferRatioCashFlow.TransferFactor)} must be between 0 and 1");

        RuleForEach(x => x.CashFlowHolder.TransferRatioCashFlows)
            .Must(x => (x.Description?.Length ?? 0) <= 50)
            .WithMessage($"{nameof(SingleCashFlow.Description)} must be less than or equal to 50 characters");

        RuleForEach(x => x.CashFlowHolder.FixedAmountCashFlows)
            .Must(x => (x.Description?.Length ?? 0) <= 50)
            .WithMessage($"{nameof(SingleCashFlow.Description)} must be less than or equal to 50 characters");
    }
}
