using System;
using PensionCoach.Tools.CommonTypes.Tax;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;

public record FixedTransferAmountDefinition : ICompositeCashFlowDefinition
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
    /// Amount which is transferred from the source to the target
    /// </summary>
    public decimal TransferAmount { get; set; }

    public FlowPair Flow { get; set; }

    public bool IsTaxable { get; set; }

    public TaxType TaxType { get; set; }
}
