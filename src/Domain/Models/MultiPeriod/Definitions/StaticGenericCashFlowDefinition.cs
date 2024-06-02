using Domain.Contracts;
using PensionCoach.Tools.CommonTypes.Tax;

namespace Domain.Models.MultiPeriod.Definitions
{
    public record StaticGenericCashFlowDefinition : IStaticCashFlowDefinition
    {
        /// <summary>
        /// Gets or sets the header properties
        /// </summary>
        public CashFlowHeader Header { get; set; }

        /// <summary>
        /// Gets or sets the date of process.
        /// </summary>
        /// <value>
        /// The date of process.
        /// </value>
        public DateTime DateOfProcess { get; set; }

        /// <summary>
        /// Gets or sets the initial amount.
        /// </summary>
        /// <value>
        /// The initial amount.
        /// </value>
        public decimal InitialAmount { get; set; }

        /// <summary>
        /// Gets the recurring investment.
        /// </summary>
        /// <value>
        /// The recurring investment.
        /// </value>
        public RecurringInvestment RecurringInvestment { get; set; }

        /// <summary>
        /// Gets or sets the net growth rate.
        /// </summary>
        /// <value>
        /// The net growth rate.
        /// </value>
        public decimal NetGrowthRate { get; set; }

        /// <summary>
        /// Get or sets the investment period beginning with the begin year and repeating count times.
        /// </summary>
        /// <value>
        /// The investment period.
        /// </value>
        public InvestmentPeriod InvestmentPeriod { get; set; }

        /// <summary>
        /// Gets the flow. Source account is cleared and moved to target account.
        /// </summary>
        /// <value>
        /// The flow.
        /// </value>
        public FlowPair Flow { get; set; }

        public bool IsTaxable { get; set; }

        public TaxType TaxType { get; set; }
    }
}
