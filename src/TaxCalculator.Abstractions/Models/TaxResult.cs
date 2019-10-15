namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    public class TaxResult
    {
        public decimal TaxableIncome { get; set; }
        public decimal Rate { get; set; }
        public decimal TaxAmount => TaxableIncome * Rate;
    }
}