using System.Collections.Generic;

namespace PensionCoach.Tools.TaxComparison;

public class TransferInCapitalBenefitsScenarioModel
{
    public IReadOnlyCollection<SingleTransferInModel> TransferIns { get; set; }

    /// <summary>
    /// Gets or sets yearly net return on transfer-ins.
    /// </summary>
    public decimal NetReturnRate { get; set; }

    public bool WithCapitalBenefitWithdrawal{ get; set; }

    /// <summary>
    /// Gets or sets available capital benefits.
    /// The amount when starting withdrawals does not include the previously added transfer-ins.
    /// </summary>
    public decimal CapitalBenefitsBeforeWithdrawal { get; set; }

    public IReadOnlyCollection<SingleTransferInModel> Withdrawals { get; set; }

}
