namespace CashFlowCalculationEngine.Server.Domain.Accounts;

public class WealthAccount : GenericCashFlowAccount
{
    public decimal Balance { get; set; }

    public decimal? NetGrowthRate { get; set; }
}
