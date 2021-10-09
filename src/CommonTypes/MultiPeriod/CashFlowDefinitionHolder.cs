using System.Collections.Generic;
using System.Linq;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod
{
    public record CashFlowDefinitionHolder
    {
        /// <summary>
        /// Gets the generic cash flow definitions.
        /// </summary>
        /// <value>
        /// The generic cash flow definitions.
        /// </value>
        public IReadOnlyCollection<GenericCashFlowDefinition> GenericCashFlowDefinitions { get; set; } =
            Enumerable.Empty<GenericCashFlowDefinition>().ToList();

        /// <summary>
        /// Gets the clear action definitions.
        /// </summary>
        /// <value>
        /// The clear action definitions.
        /// </value>
        public IReadOnlyCollection<ClearAccountAction> ClearAccountActions { get; set; } =
            Enumerable.Empty<ClearAccountAction>().ToList();

        /// <summary>
        /// Gets the change residence actions.
        /// </summary>
        /// <value>
        /// The change residence actions.
        /// </value>
        public IReadOnlyCollection<ChangeResidenceAction> ChangeResidenceActions { get; set; } =
                    Enumerable.Empty<ChangeResidenceAction>().ToList();
    }
}
