namespace TaxCalculator.WebApi.Models
{
    public class TaxRateDetails
    {
        public decimal CantonRate { get; set; }

        public decimal MunicipalityRate { get; set; }

        public decimal ChurchTaxRate { get; set; }
    }
}