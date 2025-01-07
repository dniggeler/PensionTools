namespace CashFlowCalculationEngine.Server.Domain.Accounts;

public class IncomeAccount : GenericCashFlowAccount
{
    public decimal Balance { get; set; }

    public decimal? NetGrowthRate { get; set; }
}
