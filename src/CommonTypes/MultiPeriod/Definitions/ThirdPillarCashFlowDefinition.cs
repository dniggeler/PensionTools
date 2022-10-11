using System;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;

public record ThirdPillarCashFlowDefinition
{
    /// <summary>
    /// Gets or sets the header properties
    /// </summary>
    public CashFlowHeader Header { get; set; }

    /// <summary>
    /// Gets or sets the date of start.
    /// </summary>
    /// <value>
    /// The date of start.
    /// </value>
    public DateTime DateOfStart { get; set; }

    /// <summary>
    /// Gets or sets the initial amount.
    /// </summary>
    /// <value>
    /// The initial amount.
    /// </value>
    public decimal InitialAmount { get; set; }

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
    public decimal YearlyInvestmentAmount { get; set; }

    /// <summary>
    /// Gets or sets the number of investments (one per year).
    /// </summary>
    public int NumberOfInvestments { get; set; }
}
