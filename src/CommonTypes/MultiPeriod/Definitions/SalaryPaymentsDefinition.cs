namespace PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;

public record SalaryPaymentsDefinition : ICompositeCashFlowDefinition
{
    /// <summary>
    /// Gets or sets the net growth rate.
    /// </summary>
    /// <value>
    /// The net growth rate.
    /// </value>
    public decimal NetGrowthRate { get; set; }

    /// <summary>
    /// Gets or sets the yearly investment amount.
    /// </summary>
    public decimal YearlyAmount { get; set; }

    /// <summary>
    /// Gets or sets the number of investments (one per year).
    /// </summary>
    public int NumberOfInvestments { get; set; }
}
