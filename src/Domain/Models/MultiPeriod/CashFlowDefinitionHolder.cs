using Domain.Contracts;
using Domain.Models.MultiPeriod.Actions;
using Domain.Models.MultiPeriod.Definitions;

namespace Domain.Models.MultiPeriod;

public record CashFlowDefinitionHolder
{
    public IReadOnlyCollection<InvestmentPortfolioDefinition> InvestmentDefinitions { get; set; } =
        Array.Empty<InvestmentPortfolioDefinition>().ToList();

    /// <summary>
    /// List of composite cash flow definitions.
    /// A composite cash flow definition is composed by one or several more basic definitions.
    /// </summary>
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
