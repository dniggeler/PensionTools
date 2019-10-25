namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    public class TaxResult
    {
        public int CalculationYear { get; set; }
        public decimal ReferencedTaxableAmount { get; set; }
        public decimal CantonRate { get; set; }
        public decimal MunicipalityRate { get; set; }
        public decimal BaseTaxAmount { get; set; }
        public decimal MunicipalityTaxAmount => BaseTaxAmount * MunicipalityRate / 100M;
        public decimal CantonTaxAmount => BaseTaxAmount * CantonRate / 100M;
        public decimal TotalTaxAmount => MunicipalityTaxAmount + CantonTaxAmount;
    }
}