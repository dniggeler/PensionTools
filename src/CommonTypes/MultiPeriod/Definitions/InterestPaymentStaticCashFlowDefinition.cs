using System;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;

public record InterestPaymentStaticCashFlowDefinition : IStaticCashFlowDefinition
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

    /// <summary>
    /// Gets or sets the principal amount. The principal does not change over time, and
    /// it is not linked to a tax scheme.
    /// </summary>
    /// <value>
    /// The principal amount.
    /// </value>
    public decimal PrincipalAmount { get; set; }

    /// <summary>
    /// Get or sets the frequency of the interest payments.
    /// </summary>
    public FrequencyType Frequency { get; set; }

    /// <summary>
    /// Gets or sets the net growth rate.
    /// </summary>
    /// <value>
    /// The net growth rate.
    /// </value>
    public decimal NetInterestRate { get; set; }

    /// <summary>
    /// Get or sets the investment period beginning with the begin year and repeating count times.
    /// </summary>
    /// <value>
    /// The investment period.
    /// </value>
    public InvestmentPeriod InvestmentPeriod { get; set; }

    /// <summary>
    /// Gets the flow. Source account is cleared and moved to target account.
    /// </summary>
    /// <value>
    /// The flow.
    /// </value>
    public FlowPair Flow { get; set; }
}
