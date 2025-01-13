namespace CashFlowCalculationEngine.Server.Domain.CashFlows;

public class CashFlowInput
{
    // all cash flows operating on the accounts
    public FixedAmountCashFlow[] FixedAmountCashFlows { get; set; } = [];

    public TransferRatioCashFlow[] TransferRatioCashFlows { get; set; } = [];
}
