using System.Collections.Generic;

namespace PensionCoach.Tools.TaxComparison;

public class TransferInCapitalBenefitsScenarioModel
{
    public IReadOnlyCollection<SingleTransferInModel> TransferIns { get; set; }

    /// <summary>
    /// Gets or sets yearly net return on transfer-ins.
    /// </summary>
    public decimal NetReturnRate { get; set; }

    public bool WithCapitalBenefitTaxation { get; set; }

    public decimal FinalRetirementCapital { get; set; }

    public int? YearOfCapitalBenefitWithdrawal { get; set; }
}
