namespace CashFlowCalculationEngine.Server.Domain.CashFlows;

public class CashFlowHolder
{
    // all cash flows operating on the accounts
    public FixedAmountCashFlow[] FixedAmountCashFlows { get; set; } = [];

    public TransferRatioCashFlow[] TransferRatioCashFlows { get; set; } = [];
}
