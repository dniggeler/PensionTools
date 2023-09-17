using Domain.Contracts;
using Domain.Models.MultiPeriod.Definitions;
using PensionCoach.Tools.CommonTypes.Tax;

namespace Domain.Models.MultiPeriod.Actions;

public record DynamicTransferAccountAction : IDynamicCashFlowDefinition
{
    /// <summary>
    /// Gets or sets the header properties
    /// </summary>
    public CashFlowHeader Header { get; set; }

    /// <summary>
    /// Gets or sets the date of transfer.
    /// </summary>
    /// <value>
    /// The date of transfer.
    /// </value>
    public DateTime DateOfProcess { get; set; }

    /// <summary>
    /// How much in relative terms is transferred from the source to the target account.
    /// </summary>
    public decimal TransferRatio { get; set; } = decimal.One;

    public FlowPair Flow { get; set; }

    public bool IsTaxable { get; set; }

    public TaxType TaxType { get; set; }
}
