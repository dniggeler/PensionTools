using System;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;

/// <summary>
/// This class represents an investment portfolio definition. The portfolio has two cash flow streams:
/// 1. capital growth on the initial investment which is subject to wealth tax and
/// 2. interest payments on the balance which is subject to income tax.
/// </summary>
public record InvestmentPortfolioDefinition : ICompositeCashFlowDefinition
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
    /// Gets or sets the initial investment amount. The initial investment does not change over time.
    /// </summary>
    /// <value>
    /// The initial investment amount.
    /// </value>
    public decimal InitialInvestment { get; set; }

    /// <summary>
    /// Gets the recurring investment.
    /// </summary>
    /// <value>
    /// The recurring investment.
    /// </value>
    public RecurringInvestment RecurringInvestment { get; set; }

    /// <summary>
    /// Gets or sets the net income rate.
    /// </summary>
    public decimal NetIncomeRate { get; set; }

    /// <summary>
    /// Gets or sets the net rate of return of the capital.
    /// </summary>
    public decimal NetCapitalGrowthRate { get; set; }

    /// <summary>
    /// Get or sets the investment period beginning with the begin year and repeating count times.
    /// </summary>
    /// <value>
    /// The investment period.
    /// </value>
    public InvestmentPeriod InvestmentPeriod { get; set; }
}
