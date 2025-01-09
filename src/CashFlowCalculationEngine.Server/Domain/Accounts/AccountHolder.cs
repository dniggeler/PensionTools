namespace CashFlowCalculationEngine.Server.Domain.Accounts;

public class AccountHolder
{
    public IEnumerable<IncomeAccount> IncomeAccounts { get; set; } = [];
    public IEnumerable<InvestmentAccount> InvestmentAccounts { get; set; } = [];
    public IEnumerable<WealthAccount> WealthAccounts { get; set; } = [];
    public IEnumerable<OccupationalPensionAccount> OccupationalPensionAccounts { get; set; } = [];
    public ThirdPillarAccount[] ThirdPillarAccounts { get; set; } = [];
    public ExogenousAccount[] ExogenousAccounts { get; set; } = [];
}
