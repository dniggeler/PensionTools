namespace Application.Tax.Proprietary.Abstractions.Models;

public class FullTaxResult
{
    public StateTaxResult StateTaxResult { get; set; }
    public BasisTaxResult FederalTaxResult { get; set; }
    public decimal TotalTaxAmount => StateTaxResult.TotalTaxAmount + FederalTaxResult.TaxAmount;
}
