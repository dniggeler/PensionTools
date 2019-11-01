namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    public class FullTaxResult
    {
        public TaxResult StateTaxResult { get; set; }
        public BasisTaxResult FederalTaxResult { get; set; }
        public decimal TotalTaxAmount => StateTaxResult.TotalTaxAmount + FederalTaxResult.TaxAmount;
    }
}