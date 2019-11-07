namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    public class AggregatedBasisTaxResult
    {
        public BasisTaxResult IncomeTax { get; set; }
        public BasisTaxResult WealthTax { get; set; }
        public decimal Total => IncomeTax.TaxAmount + WealthTax.TaxAmount;
    }
}