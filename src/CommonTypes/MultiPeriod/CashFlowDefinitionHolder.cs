using System;
using System.Collections.Generic;
using System.Linq;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Actions;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Definitions;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod;

public record CashFlowDefinitionHolder
{
    /// <summary>
    /// Gets the generic cash flow definitions.
    /// </summary>
    /// <value>
    /// The generic cash flow definitions.
    /// </value>
    public IReadOnlyCollection<GenericCashFlowDefinition> GenericCashFlowDefinitions { get; set; } =
        Array.Empty<GenericCashFlowDefinition>().ToList();

    /// <summary>
    /// Gets the clear action definitions.
    /// </summary>
    /// <value>
    /// The clear action definitions.
    /// </value>
    public IReadOnlyCollection<ClearAccountAction> ClearAccountActions { get; set; } =
        Array.Empty<ClearAccountAction>().ToList();

    /// <summary>
    /// Gets the change residence actions.
    /// </summary>
    /// <value>
    /// The change residence actions.
    /// </value>
    public IReadOnlyCollection<ChangeResidenceAction> ChangeResidenceActions { get; set; } =
        Array.Empty<ChangeResidenceAction>().ToList();

    /// <summary>
    /// Collection of cash-flow actions.
    /// </summary>
    public IReadOnlyCollection<ICashFlowAction> CashFlowActions { get; set; } =
        Array.Empty<ICashFlowAction>().ToList();
}
