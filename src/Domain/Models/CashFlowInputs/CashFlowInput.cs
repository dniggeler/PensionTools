namespace Domain.Models.CashFlowInputs;

public class CashFlowInput
{
    // all cash flows operating on the accounts
    public FixedAmountCashFlowInput[] FixedAmountCashFlows { get; set; } = [];

    public TransferRatioCashFlowInput[] TransferRatioCashFlows { get; set; } = [];

    public BalanceGrowthCashFlowInput[] BalanceGrowthCashFlows { get; set; } = [];
}
