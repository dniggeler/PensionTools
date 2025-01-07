namespace CashFlowCalculationEngine.Server.Domain
{
    public record AccountTransaction(string Description, DateTime? ValutaDate, decimal? Amount);
}
