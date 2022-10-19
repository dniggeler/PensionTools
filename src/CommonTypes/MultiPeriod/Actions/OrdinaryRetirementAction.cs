namespace PensionCoach.Tools.CommonTypes.MultiPeriod.Actions;

public record OrdinaryRetirementAction : ICashFlowAction
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

    public int NumberOfPeriods { get; set; }

    public decimal CapitalOptionFactor { get; set; } = decimal.Zero;

    public decimal RetirementPension { get; set; }

    public decimal CapitalConsumptionAmountPerYear { get; set; }

    public decimal AhvPensionAmount { get; set; }
}
