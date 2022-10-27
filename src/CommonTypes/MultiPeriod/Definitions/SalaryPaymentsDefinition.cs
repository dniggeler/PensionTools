using System;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Actions;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;

public record SalaryPaymentsDefinition : ICashFlowDefinition
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
    public DateTime DateOfProcess { get; set; }

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
