namespace Application.Tax.Proprietary.Abstractions.Models
{
    public class SingleTaxResult
    {
        public BasisTaxResult BasisTaxAmount { get; set; }
        public decimal CantonRate { get; set; }
        public decimal MunicipalityRate { get; set; }
        public decimal MunicipalityTaxAmount => MunicipalityRate / 100M * BasisTaxAmount.TaxAmount;
        public decimal CantonTaxAmount => CantonRate / 100M * BasisTaxAmount.TaxAmount;
        public decimal TotalTaxAmount => MunicipalityTaxAmount + CantonTaxAmount;
    }
}
