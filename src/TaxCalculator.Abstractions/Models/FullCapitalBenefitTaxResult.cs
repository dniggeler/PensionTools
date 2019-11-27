namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    public class FullCapitalBenefitTaxResult
    { 
        public CapitalBenefitTaxResult StateResult { get; set; }
        public BasisTaxResult FederalResult { get; set; }
        public decimal TotalTaxAmount => StateResult.TotalTaxAmount + FederalResult.TaxAmount;
    }
}