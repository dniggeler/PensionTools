using System;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;

public record SetupAccountDefinition
{
    /// <summary>
    /// Gets or sets the date of start.
    /// </summary>
    /// <value>
    /// The date of start.
    /// </value>
    public DateTime DateOfStart { get; set; }

    /// <summary>
    /// Gets or sets the initial wealth.
    /// </summary>
    /// <value>
    /// The initial wealth.
    /// </value>
    public decimal InitialWealth { get; set; }

    /// <summary>
    /// Gets or sets the initial capital benefits.
    /// </summary>
    public decimal InitialCapitalBenefits { get; set; }

}
