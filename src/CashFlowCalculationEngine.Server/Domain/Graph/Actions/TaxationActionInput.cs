using Domain.Enums;

namespace CashFlowCalculationEngine.Server.Domain.Graph.Actions;

public class TaxationActionInput
{
    public string? Description { get; set; }

    public ProcessDateKind KindOfProcessDate { get; set; } 

    public DateOnly? DateOfProcess { get; set; }
    
    public int? Sequence { get; set; }

    public Guid TaxDestinationAccountId { get; set; }

    public decimal TaxFactor { get; set; }

    public TaxType TaxType { get; set; }
}
