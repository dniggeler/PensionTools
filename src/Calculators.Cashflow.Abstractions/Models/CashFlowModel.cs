namespace Calculators.CashFlow.Models
{
    public record CashFlowModel(int Year, decimal Amount, FundsType Source, FundsType Target);
}
