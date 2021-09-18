using System.Collections.Generic;
using System.Linq;

namespace Calculators.CashFlow.Models
{
    public record CashFlowDefinitionHolder
    {
        /// <summary>
        /// Gets the generic cash flow definitions.
        /// </summary>
        /// <value>
        /// The generic cash flow definitions.
        /// </value>
        public IReadOnlyCollection<GenericCashFlowDefinition> GenericCashFlowDefinitions { get; init; } =
            Enumerable.Empty<GenericCashFlowDefinition>().ToList();

        /// <summary>
        /// Gets the clear action definitions.
        /// </summary>
        /// <value>
        /// The clear action definitions.
        /// </value>
        public IReadOnlyCollection<ClearActionDefinition> ClearCashFlowDefinitions { get; init; } =
            Enumerable.Empty<ClearActionDefinition>().ToList();
    }
}
