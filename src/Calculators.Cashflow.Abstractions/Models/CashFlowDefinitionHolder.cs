using System.Collections.Generic;

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
        public IReadOnlyCollection<GenericCashFlowDefinition> GenericCashFlowDefinitions { get; init; }

        /// <summary>
        /// Gets the clear cash flow definitions.
        /// </summary>
        /// <value>
        /// The clear cash flow definitions.
        /// </value>
        public IReadOnlyCollection<ClearCashFlowDefinition> ClearCashFlowDefinitions { get; init; }
    }
}
