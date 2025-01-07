namespace CashFlowCalculationEngine.Server.Domain.Accounts;

public class OccupationalPensionAccount : GenericCashFlowAccount
{
    public decimal Balance { get; set; }

    public decimal NetGrowthRate { get; set; }
}
