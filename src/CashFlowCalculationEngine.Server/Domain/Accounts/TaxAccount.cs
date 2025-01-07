namespace CashFlowCalculationEngine.Server.Domain.Accounts;

public class TaxAccount : GenericCashFlowAccount
{
    public decimal Balance { get; set; }

    public decimal NetGrowthRate { get; set; }
}
