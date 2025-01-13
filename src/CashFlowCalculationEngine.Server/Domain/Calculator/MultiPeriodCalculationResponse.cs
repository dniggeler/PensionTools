namespace CashFlowCalculationEngine.Server.Domain.Calculator;

public record MultiPeriodCalculationResponse
{
    public Guid CalculationId { get; set; }

    public AccountTransactionResponse? Transactions { get; set; }
}
