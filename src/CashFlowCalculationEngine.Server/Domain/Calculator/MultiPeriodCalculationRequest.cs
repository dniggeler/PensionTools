using CashFlowCalculationEngine.Server.Domain.Accounts;
using CashFlowCalculationEngine.Server.Domain.CashFlows;
using CashFlowCalculationEngine.Server.Domain.Location;
using CashFlowCalculationEngine.Server.Domain.Person;

namespace CashFlowCalculationEngine.Server.Domain.Calculator;

public class MultiPeriodCalculationRequest
{
    public Guid CalculationId { get; set; }

    public CalculationParameters CalculationParameter { get; set; } = new();

    public CalculationPerson Person { get; set; } = new();

    public Municipality Municipality { get; set; } = new();

    public AccountHolder AccountHolder { get; set; } = new();

    public CashFlowHolder CashFlowHolder { get; set; } = new();
}
