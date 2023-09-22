using Domain.Models.Cashflows;

namespace Domain.Models.MultiPeriod;

public record MultiPeriodCalculationResult
{
    public int StartingYear { get; set; }
        
    public int NumberOfPeriods { get; set; }

    public IEnumerable<SinglePeriodCalculationResult> Accounts{ get; set; }

    public AccountTransactionResultHolder Transactions { get; set; }
}
