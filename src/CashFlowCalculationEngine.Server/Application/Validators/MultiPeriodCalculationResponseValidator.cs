using CashFlowCalculationEngine.Server.Domain.Calculator;
using FluentValidation;

namespace CashFlowCalculationEngine.Server.Application.Validators;

public class MultiPeriodCalculationResponseValidator : AbstractValidator<MultiPeriodCalculationResponse>
{
    public MultiPeriodCalculationResponseValidator()
    {
        RuleFor(x => x.Transactions)
            .SetValidator(new AccountTransactionValidator());
    }
}

