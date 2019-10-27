namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    public class BasisTaxResult
    {
        public int CalculationYear { get; set; }
        public decimal DeterminingFactorTaxableAmount { get; set; }
        public decimal TaxAmount { get; set; }
    }
}