using Domain.Models.Cashflows;
using Domain.Models.MultiPeriod;

namespace CashFlowCalculationEngine.Server.Domain;

public record MultiPeriodCalculationResponse
{
    public int StartingYear { get; set; }
        
    public int NumberOfPeriods { get; set; }

    public IEnumerable<SinglePeriodCalculationResult> Accounts { get; set; } = [];

    public AccountTransactionResultHolder? Transactions { get; set; }
}
