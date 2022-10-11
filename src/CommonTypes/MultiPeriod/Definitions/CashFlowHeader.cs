namespace PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;

public record CashFlowHeader
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
}
