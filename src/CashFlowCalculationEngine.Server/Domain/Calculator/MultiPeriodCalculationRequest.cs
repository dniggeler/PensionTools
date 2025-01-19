using CashFlowCalculationEngine.Server.Domain.CashFlows;
using CashFlowCalculationEngine.Server.Domain.Graph.Accounts;
using CashFlowCalculationEngine.Server.Domain.Graph.Actions;
using CashFlowCalculationEngine.Server.Domain.Location;
using CashFlowCalculationEngine.Server.Domain.Person;

namespace CashFlowCalculationEngine.Server.Domain.Calculator;

public class MultiPeriodCalculationRequest
{
    public Guid CalculationId { get; set; }

    public CalculationParameters CalculationParameter { get; set; } = new();

    public CalculationPerson Person { get; set; } = new();

    public Municipality Municipality { get; set; } = new();

    public AccountInput AccountHolder { get; set; } = new();

    public CashFlowInput CashFlowHolder { get; set; } = new();

    public TaxActionInput TaxationActionHolder { get; set; } = new();
}
