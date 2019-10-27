namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    public class BasisTaxResult
    {
        public decimal DeterminingFactorTaxableAmount { get; set; }
        public decimal TaxAmount { get; set; }
    }
}