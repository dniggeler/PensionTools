namespace Application.Bvg.Models;

public class PredecessorRetirementCapital
{
    /// <summary>
    /// Gets or sets the process date.
    /// </summary>
    /// <value>
    /// The process date.
    /// </value>
    public DateTime DateOfProcess { get; set; }

    /// <summary>
    /// Gets or sets the begin of year amount.
    /// </summary>
    /// <value>
    /// The begin of year amount.
    /// </value>
    public decimal BeginOfYearAmount { get; set; }

    /// <summary>
    /// Gets or sets the end of year amount.
    /// </summary>
    /// <value>
    /// The end of year amount.
    /// </value>
    public decimal EndOfYearAmount { get; set; }

    /// <summary>
    /// Gets or sets the current amount.
    /// </summary>
    /// <value>
    /// The current amount.
    /// </value>
    public decimal CurrentAmount { get; set; }
}
