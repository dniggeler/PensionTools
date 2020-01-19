namespace TaxCalculator.WebApi.Models
{
    public class TaxAmountDetail
    {
        public decimal FederalTaxAmount { get; set; }

        public decimal MunicipalityTaxAmount { get; set; }

        public decimal CantonTaxAmount { get; set; }

        public decimal ChurchTaxAmount { get; set; }
    }
}