using Domain.Models.Cashflows;
using Domain.Models.MultiPeriod;

namespace CashFlowCalculationEngine.Server.Domain.Calculator;

public record MultiPeriodCalculationResult
{
    public IEnumerable<SinglePeriodCalculationResult> Accounts{ get; set; }

    public AccountTransactionResultHolder Transactions { get; set; }
}
