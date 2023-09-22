using Domain.Models.Tax;

namespace PensionCoach.Tools.CommonTypes.Tax;

public class CapitalBenefitTaxResponse
{
    public string Name { get; set; }

    public int CalculationYear { get; set; }

    public decimal TotalTaxAmount { get; set; }

    public TaxAmountDetail TaxDetails { get; set; }
}
