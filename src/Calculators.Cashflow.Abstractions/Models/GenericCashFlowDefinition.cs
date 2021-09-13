using System;
using PensionCoach.Tools.CommonTypes;

namespace Calculators.CashFlow.Models
{
    public record GenericCashFlowDefinition
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; init; }

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
        /// Gets the step amount.
        /// </summary>
        /// <value>
        /// The step amount.
        /// </value>
        public (decimal Amount, FrequencyType Frequency) RecurringAmount { get; init; }

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
        public (int BeginYear, int Count) InvestmentPeriod { get; init; }

        /// <summary>
        /// Get or sets the period the cash-flow is producing values.
        /// </summary>
        /// <value>
        /// The net growth rate.
        /// </value>
        public (AccountType Source, AccountType Target) Flow { get; init; }

        public bool IsTaxable { get; set; }

        public TaxType TaxType { get; set; }

        public OccurrenceType OccurrenceType { get; set; }
    }
}
