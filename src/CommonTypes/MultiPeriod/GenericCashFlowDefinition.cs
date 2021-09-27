using PensionCoach.Tools.CommonTypes.Tax;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod
{
    public record GenericCashFlowDefinition
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
        /// Gets or sets the initial amount.
        /// </summary>
        /// <value>
        /// The initial amount.
        /// </value>
        public decimal InitialAmount { get; init; }

        /// <summary>
        /// Gets the recurring investment.
        /// </summary>
        /// <value>
        /// The recurring investment.
        /// </value>
        public RecurringInvestment RecurringInvestment { get; init; }

        /// <summary>
        /// Gets or sets the net growth rate.
        /// </summary>
        /// <value>
        /// The net growth rate.
        /// </value>
        public decimal NetGrowthRate { get; init; }

        /// <summary>
        /// Get or sets the investment period beginning with the begin year and repeating count times.
        /// </summary>
        /// <value>
        /// The investment period.
        /// </value>
        public InvestmentPeriod InvestmentPeriod { get; init; }

        /// <summary>
        /// Gets the flow. Source account is cleared and moved to target account.
        /// </summary>
        /// <value>
        /// The flow.
        /// </value>
        public FlowPair Flow { get; init; }

        public bool IsTaxable { get; set; }

        public TaxType TaxType { get; set; }

        public OccurrenceType OccurrenceType { get; set; }
    }
}
