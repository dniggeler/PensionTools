using PensionCoach.Tools.CommonTypes;

namespace Calculators.CashFlow.Models
{
    public record ClearActionDefinition
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; init; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; init; }

        /// <summary>
        /// Gets or sets the ordinal to define a linear order between multiple cash-flows.
        /// </summary>
        /// <value>
        /// The ordinal.
        /// </value>
        public int Ordinal { get; init; }

        /// <summary>
        /// Gets the clear at year.
        /// </summary>
        /// <value>
        /// The clear at year.
        /// </value>
        public int ClearAtYear { get; init; }

        public decimal ClearRatio { get; init; } = decimal.One;

        public FlowPair Flow { get; init; }

        public bool IsTaxable { get; set; }

        public TaxType TaxType { get; set; }

        public OccurrenceType OccurrenceType { get; set; }
    }
}
