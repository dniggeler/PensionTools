namespace CashFlowCalculationEngine.Server.Domain.Graph.Actions;

public class TaxActionInput
{
    public Guid TaxPaymentSourceAccountId { get; set; }

    public Guid TaxPaymentTargetAccountId { get; set; }

    public TaxBalanceAction[] BalanceActions { get; set; } = [];
}
