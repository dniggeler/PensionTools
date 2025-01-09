using CashFlowCalculationEngine.Server.Domain.Accounts;
using CashFlowCalculationEngine.Server.Domain.CashFlows;

namespace CashFlowCalculationEngine.Server.Domain;

public class MultiPeriodCalculationRequest
{
    public AccountHolder AccountHolder { get; set; } = new AccountHolder();

    public CashFlowHolder CashFlowHolder { get; set; } = new CashFlowHolder();
}
