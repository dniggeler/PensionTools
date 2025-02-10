using AppAny.HotChocolate.FluentValidation;
using CashFlowCalculationEngine.Server.Application.Calculators;
using CashFlowCalculationEngine.Server.Application.Validators;
using CashFlowCalculationEngine.Server.Domain.Calculator;
using FluentValidation.Results;

namespace CashFlowCalculationEngine.Server.Queries;

public sealed class MultiPeriodCashFlowQueries(
    IMultiPeriodCashFlowCalculator calculator,
    MultiPeriodCalculationResponseValidator responseValidator)
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
            request.TaxationActionHolder,
            cancellationToken);

        var response = result with { IsSuccess = true, CalculationId = request.CalculationId };

        ValidationResult? validationResult = responseValidator.Validate(response);

        if (!validationResult.IsValid)
        {
            response.IsSuccess = false;
            response.Errors = validationResult.Errors.Select(f => f.ErrorMessage).ToArray();
        }

        return response;
    }
}
