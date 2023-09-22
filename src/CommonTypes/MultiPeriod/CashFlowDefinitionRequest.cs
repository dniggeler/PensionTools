using System;
using System.Collections.Generic;
using Domain.Models.MultiPeriod.Actions;
using Domain.Models.MultiPeriod.Definitions;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod
{
    public record CashFlowDefinitionRequest
    {
        public FixedTransferAmountDefinition FixedTransferAmountDefinition { get; set; }

        public OrdinaryRetirementAction OrdinaryRetirementAction { get; set; }

        public PurchaseInsuranceYearsPaymentsDefinition PurchaseInsuranceYearsPaymentsDefinition { get; set; }

        public RelativeTransferAmountDefinition RelativeTransferAmountDefinition { get; set; }

        public SalaryPaymentsDefinition SalaryPaymentsDefinition { get; set; }

        public SetupAccountDefinition SetupAccountDefinition { get; set; }

        public ThirdPillarPaymentsDefinition ThirdPillarPaymentsDefinition { get; set; }

        /// <summary>
        /// Gets the static generic cash flow definitions.
        /// </summary>
        /// <value>
        /// The generic cash flow definitions.
        /// </value>
        public IEnumerable<StaticGenericCashFlowDefinition> StaticGenericCashFlowDefinitions { get; set; } =
            Array.Empty<StaticGenericCashFlowDefinition>();

        /// <summary>
        /// Gets the static generic cash flow definitions.
        /// </summary>
        public IEnumerable<DynamicGenericCashFlowDefinition> DynamicGenericCashFlowDefinitions { get; set; } =
            Array.Empty<DynamicGenericCashFlowDefinition>();

        public DynamicTransferAccountAction DynamicTransferAccountAction { get; set; }

        /// <summary>
        /// Gets the change residence actions.
        /// </summary>
        /// <value>
        /// The change residence actions.
        /// </value>
        public IEnumerable<ChangeResidenceAction> ChangeResidenceActions { get; set; } =
            Array.Empty<ChangeResidenceAction>();
    }
}
