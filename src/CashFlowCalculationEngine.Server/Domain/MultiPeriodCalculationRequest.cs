using CashFlowCalculationEngine.Server.Domain.Accounts;

namespace CashFlowCalculationEngine.Server.Domain;

public class MultiPeriodCalculationRequest
{
    public IEnumerable<IncomeAccount>? IncomeAccounts { get; set; }

    public IEnumerable<InvestmentAccount>? InvestmentAccounts { get; set; }

    public IEnumerable<WealthAccount>? WealthAccounts { get; set; }

    public IEnumerable<OccupationalPensionAccount>? OccupationalPensionAccounts { get; set; }
    
    public ThirdPillarAccount[]? ThirdPillarAccounts { get; set; }
}
