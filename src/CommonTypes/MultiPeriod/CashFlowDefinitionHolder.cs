using System;
using System.Collections.Generic;
using System.Linq;
using PensionCoach.Tools.CommonTypes.MultiPeriod.Actions;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod;

public record CashFlowDefinitionHolder
{
    /// <summary>
    /// Gets the change residence actions.
    /// </summary>
    /// <value>
    /// The change residence actions.
    /// </value>
    public IReadOnlyCollection<ICompositeCashFlowDefinition> Composites { get; set; } =
        Array.Empty<ICompositeCashFlowDefinition>().ToList();

    /// <summary>
    /// Gets the generic cash flow definitions.
    /// </summary>
    /// <value>
    /// The generic cash flow definitions.
    /// </value>
    public IReadOnlyCollection<IStaticCashFlowDefinition> StaticGenericCashFlowDefinitions { get; set; } =
        Array.Empty<IStaticCashFlowDefinition>().ToList();

    /// <summary>
    /// Collection of cash-flow actions.
    /// </summary>
    public IReadOnlyCollection<ICashFlowDefinition> CashFlowActions { get; set; } =
        Array.Empty<IStaticCashFlowDefinition>().ToList();

    /// <summary>
    /// Gets the change residence actions.
    /// </summary>
    /// <value>
    /// The change residence actions.
    /// </value>
    public IReadOnlyCollection<ChangeResidenceAction> ChangeResidenceActions { get; set; } =
        Array.Empty<ChangeResidenceAction>().ToList();
}
