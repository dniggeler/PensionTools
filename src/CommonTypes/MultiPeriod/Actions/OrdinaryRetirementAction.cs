using System;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod.Actions;

public record OrdinaryRetirementAction : ICashFlowDefinition
{
    /// <summary>
    /// Gets or sets the header properties
    /// </summary>
    public CashFlowHeader Header { get; set; }

    public DateTime DateOfProcess { get; set; }

    public int NumberOfPeriods { get; set; }

    public decimal CapitalOptionFactor { get; set; } = decimal.Zero;

    public decimal RetirementPension { get; set; }

    public decimal CapitalConsumptionAmountPerYear { get; set; }

    public decimal AhvPensionAmount { get; set; }
}
