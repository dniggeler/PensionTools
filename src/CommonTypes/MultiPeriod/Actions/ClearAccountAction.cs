using System;
using PensionCoach.Tools.CommonTypes.Tax;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod.Actions;

public record ClearAccountAction
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
    /// Gets or sets the ordinal to define a linear order between multiple cash-flows.
    /// </summary>
    /// <value>
    /// The ordinal.
    /// </value>
    public int Ordinal { get; set; }

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
