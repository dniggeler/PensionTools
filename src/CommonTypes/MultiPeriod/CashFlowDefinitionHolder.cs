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
        Array.Empty<ICashFlowDefinition>().ToList();

    /// <summary>
    /// Gets the transfer action definitions.
    /// </summary>
    /// <value>
    /// The transfer action definitions.
    /// </value>
    public IReadOnlyCollection<ICashFlowDefinition> TransferAccountActions { get; set; } =
        Array.Empty<StaticTransferAccountAction>().ToList();

    /// <summary>
    /// Gets the change residence actions.
    /// </summary>
    /// <value>
    /// The change residence actions.
    /// </value>
    public IReadOnlyCollection<ICompositeCashFlowDefinition> Composites { get; set; } =
        Array.Empty<ICompositeCashFlowDefinition>().ToList();

    /// <summary>
    /// Collection of cash-flow actions.
    /// </summary>
    public IReadOnlyCollection<ICashFlowDefinition> CashFlowActions { get; set; } =
        Array.Empty<IStaticCashFlowDefinition>().ToList();
}
