using System;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Actions;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;

public record SetupAccountDefinition : ICashFlowDefinition
{
    public CashFlowHeader Header { get; set; }

    /// <inheritdoc />
    public DateTime DateOfProcess { get; set; }

    /// <summary>
    /// Gets or sets the initial wealth.
    /// </summary>
    /// <value>
    /// The initial wealth.
    /// </value>
    public decimal InitialWealth { get; set; }

    /// <summary>
    /// Gets or sets the initial occupational pension assets (Berufliche Vorsorge).
    /// </summary>
    public decimal InitialOccupationalPensionAssets { get; set; }

    /// <summary>
    /// Gets or sets the initial third pillar assets (3A Konto).
    /// </summary>
    public decimal InitialThirdPillarAssets { get; set; }
}
