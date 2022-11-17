using System;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod;

public interface ICashFlowDefinition
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
}
