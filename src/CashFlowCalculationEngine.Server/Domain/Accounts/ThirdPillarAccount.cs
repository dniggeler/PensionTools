namespace CashFlowCalculationEngine.Server.Domain.Accounts;

public class ThirdPillarAccount : GenericCashFlowAccount
{
    public decimal Balance { get; set; }

    public decimal NetGrowthRate { get; set; }
}
