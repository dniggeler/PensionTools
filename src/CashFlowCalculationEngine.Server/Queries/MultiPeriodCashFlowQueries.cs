using AppAny.HotChocolate.FluentValidation;
using CashFlowCalculationEngine.Server.Application.Calculators;
using CashFlowCalculationEngine.Server.Application.Validators;
using CashFlowCalculationEngine.Server.Domain.Calculator;

namespace CashFlowCalculationEngine.Server.Queries;

public sealed class MultiPeriodCashFlowQueries(IMultiPeriodCashFlowCalculator calculator)
{
    public async Task<MultiPeriodCalculationResponse> CalculateAsync(
        [UseFluentValidation, UseValidator<MultiPeriodCalculationRequestValidator>]MultiPeriodCalculationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await calculator.CalculateAsync(
            request.CalculationParameter,
            request.Person,
            request.Municipality,
            request.AccountHolder, 
            request.CashFlowHolder,
            request.TaxationActions,
            cancellationToken);

        var response = new MultiPeriodCalculationResponse
        {
            CalculationId = request.CalculationId,
            Transactions = result.Transactions
        };

        return response;
    }
}
