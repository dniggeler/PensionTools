namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    public class TaxResult
    {
        public decimal TaxableIncome { get; set; }
        public decimal CantonRate { get; set; }
        public decimal MunicipalityRate { get; set; }
        public decimal BaseTaxAmount { get; set; }
        public decimal MunicipalityTaxAmount => TaxableIncome * MunicipalityRate;
        public decimal CantonTaxAmount => TaxableIncome * CantonRate;
        public decimal TotalTaxAmount => MunicipalityTaxAmount + CantonRate;
    }
}