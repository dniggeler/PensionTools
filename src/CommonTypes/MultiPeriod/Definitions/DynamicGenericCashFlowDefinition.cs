using System;
using PensionCoach.Tools.CommonTypes.Tax;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;

public record DynamicGenericCashFlowDefinition : IStaticCashFlowDefinition
{
    /// <summary>
    /// Gets or sets the header properties
    /// </summary>
    public CashFlowHeader Header { get; set; }

    /// <summary>
    /// Gets or sets the date of process.
    /// </summary>
    /// <value>
    /// The date of process.
    /// </value>
    public DateTime DateOfProcess { get; set; }

    public int NumberOfPeriods { get; set; }

    /// <summary>
    /// Fraction of the available capital which is transferred from the source to the target account.
    /// </summary>
    public decimal TransferQuota { get; set; }

    public FrequencyType Frequency { get; set; }

    /// <summary>
    /// Gets the flow. Source account is cleared and moved to target account.
    /// </summary>
    /// <value>
    /// The flow.
    /// </value>
    public FlowPair Flow { get; set; }

    public bool IsTaxable { get; set; }

    public TaxType TaxType { get; set; }
}
