using System;
using PensionCoach.Tools.CommonTypes.Tax;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod.Actions;

public record ClearAccountAction : ICashFlowAction
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public string Id { get; set; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; }

    /// <summary>
    /// Gets the clear at year.
    /// </summary>
    /// <value>
    /// The clear at year.
    /// </value>
    public DateTime DateOfClearing { get; set; }

    public decimal ClearRatio { get; set; } = decimal.One;

    public FlowPair Flow { get; set; }

    public bool IsTaxable { get; set; }

    public TaxType TaxType { get; set; }

    public OccurrenceType OccurrenceType { get; set; }
}
