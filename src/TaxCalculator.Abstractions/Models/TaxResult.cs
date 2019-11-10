using LanguageExt;


namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    public class TaxResult
    {
        public int CalculationYear { get; set; }
        public BasisTaxResult BasisIncomeTax { get; set; }
        public BasisTaxResult BasisWealthTax { get; set; }
        public ChurchTaxResult ChurchTax { get; set; }
        public decimal CantonRate { get; set; }
        public decimal MunicipalityRate { get; set; }
        public decimal MunicipalityTaxAmount => MunicipalityRate / 100M * (BasisIncomeTax.TaxAmount + BasisWealthTax.TaxAmount);
        public decimal CantonTaxAmount => CantonRate / 100M * (BasisIncomeTax.TaxAmount + BasisWealthTax.TaxAmount);
        public decimal TotalTaxAmount => MunicipalityTaxAmount + CantonTaxAmount;
        public Option<decimal> PollTaxAmount { get; set; }
    }
}