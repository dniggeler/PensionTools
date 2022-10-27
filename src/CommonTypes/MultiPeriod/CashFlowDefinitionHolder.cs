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
    public IReadOnlyCollection<ICashFlowDefinition> GenericCashFlowDefinitions { get; set; } =
        Array.Empty<GenericCashFlowDefinition>().ToList();

    /// <summary>
    /// Gets the transfer action definitions.
    /// </summary>
    /// <value>
    /// The transfer action definitions.
    /// </value>
    public IReadOnlyCollection<ICashFlowDefinition> TransferAccountActions { get; set; } =
        Array.Empty<TransferAccountAction>().ToList();

    /// <summary>
    /// Gets the change residence actions.
    /// </summary>
    /// <value>
    /// The change residence actions.
    /// </value>
    public IReadOnlyCollection<ICashFlowDefinition> ChangeResidenceActions { get; set; } =
        Array.Empty<ChangeResidenceAction>().ToList();

    /// <summary>
    /// Collection of cash-flow actions.
    /// </summary>
    public IReadOnlyCollection<ICashFlowDefinition> CashFlowActions { get; set; } =
        Array.Empty<ICashFlowDefinition>().ToList();
}
