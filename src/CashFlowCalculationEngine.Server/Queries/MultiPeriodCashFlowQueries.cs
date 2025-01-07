using CashFlowCalculationEngine.Server.Domain;
using CashFlowCalculationEngine.Server.Domain.Accounts;

namespace CashFlowCalculationEngine.Server.Queries;

public sealed class MultiPeriodCashFlowQueries
{
    public IEnumerable<GenericCashFlowAccount> CalculateAsync(MultiPeriodCalculationRequest request)
    {
        var result = new MultiPeriodCalculationResponse()
        {
        };

        return [
            new IncomeAccount
            {
                Id = Guid.NewGuid(),
                Balance = 5000,
            },
            new ExogenousAccount
            {
                Id = Guid.NewGuid(),
                Balance = 1000,
            }
        ];
    }
}
