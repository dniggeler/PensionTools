namespace CashFlowCalculationEngine.Server.Domain.Accounts;

public class WealthAccount : GenericCashFlowAccount
{
    public decimal? NetGrowthRate { get; set; }
}
