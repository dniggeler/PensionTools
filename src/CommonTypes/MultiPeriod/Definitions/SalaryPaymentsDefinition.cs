using System;

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
    /// Gets or sets the end of period. It marks not the final salary payment
    /// but is used the calculate the pro-rated yearly amount in the last payment period.
    /// </summary>
    public DateTime DateOfEndOfPeriod { get; set; }
}
