namespace CashFlowCalculationEngine.Server.Domain;

public class SinglePeriodCalculationResponse
{
    public int Year { get; set; }
    
    public decimal Amount { get; set; }

    public Guid AccountId { get; set; }
}
