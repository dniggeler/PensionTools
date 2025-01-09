using CashFlowCalculationEngine.Server.Domain.Accounts;
using CashFlowCalculationEngine.Server.Domain.Calculator;
using CashFlowCalculationEngine.Server.Domain.CashFlows;
using CashFlowCalculationEngine.Server.Domain.Location;
using CashFlowCalculationEngine.Server.Domain.Person;

namespace CashFlowCalculationEngine.Server.Application.Calculators;

public class MultiPeriodCashFlowCalculator : IMultiPeriodCashFlowCalculator
{
    public Task<MultiPeriodCalculationResult> CalculateAsync(CalculationParameters calculationParameters, CalculationPerson person, Municipality municipality, AccountHolder accountHolder,
        CashFlowHolder cashFlowHolder, CancellationToken cancellationToken)
    {
        var result = new MultiPeriodCalculationResult();

        return Task.FromResult(result);
    }
}
