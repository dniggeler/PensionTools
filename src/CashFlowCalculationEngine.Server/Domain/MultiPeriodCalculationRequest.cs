using CashFlowCalculationEngine.Server.Domain.Accounts;
using CashFlowCalculationEngine.Server.Domain.CashFlows;

namespace CashFlowCalculationEngine.Server.Domain;

public class MultiPeriodCalculationRequest
{
    // all accounts
    public IEnumerable<IncomeAccount>? IncomeAccounts { get; set; }

    public IEnumerable<InvestmentAccount>? InvestmentAccounts { get; set; }

    public IEnumerable<WealthAccount>? WealthAccounts { get; set; }

    public IEnumerable<OccupationalPensionAccount>? OccupationalPensionAccounts { get; set; }
    
    public ThirdPillarAccount[]? ThirdPillarAccounts { get; set; }

    public ExogenousAccount[]? ExogenousAccounts { get; set; }

    // all cash flows operating on the accounts
    public SingleCashFlow[] CashFlows { get; set; } = [];
}
