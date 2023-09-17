using System;
using Domain.Enums;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod.Actions;

/// <summary>
/// Simulates the change of the residence. A change always happens at the end of a period.
/// </summary>
public record ChangeResidenceAction : ICompositeCashFlowDefinition
{
    /// <summary>
    /// Gets or sets the header properties
    /// </summary>
    public CashFlowHeader Header { get; set; }

    /// <summary>
    /// Gets the date of change of residence.
    /// </summary>
    /// <value>
    /// The change of residence at this year.
    /// </value>
    public DateTime DateOfProcess { get; set; }

    /// <summary>
    /// Gets the destination municipality identifier.
    /// </summary>
    /// <value>
    /// The destination municipality identifier.
    /// </value>
    public int DestinationMunicipalityId { get; set; }

    /// <summary>
    /// Gets or sets the destination canton.
    /// </summary>
    /// <value>
    /// The destination canton.
    /// </value>
    public Canton DestinationCanton { get; set; }

    /// <summary>
    /// Gets the change cost. The cost to change of the residence is deducted from wealth.
    /// </summary>
    /// <value>
    /// The change cost.
    /// </value>
    public decimal ChangeCost { get; set; }
}
