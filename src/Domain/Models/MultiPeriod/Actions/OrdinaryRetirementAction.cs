using Domain.Contracts;
using Domain.Models.MultiPeriod.Definitions;

namespace Domain.Models.MultiPeriod.Actions
{
    public record OrdinaryRetirementAction : ICompositeCashFlowDefinition
    {
        /// <summary>
        /// Gets or sets the header properties
        /// </summary>
        public CashFlowHeader Header { get; set; }

        public DateTime DateOfProcess { get; set; }

        public int NumberOfPeriods { get; set; }

        public decimal CapitalOptionFactor { get; set; } = decimal.Zero;

        public decimal RetirementPension { get; set; }

        public decimal CapitalConsumptionAmountPerYear { get; set; }

        public decimal AhvPensionAmount { get; set; }
    }
}
