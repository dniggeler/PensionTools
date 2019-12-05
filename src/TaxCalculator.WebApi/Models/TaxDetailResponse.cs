namespace TaxCalculator.WebApi.Models
{
    public class TaxDetailResponse
    {
        public decimal FederalTaxAmount { get; set; }

        public decimal MunicipalityTaxAmount { get; set; }

        public decimal StateTaxAmount { get; set; }

        public decimal ChurchTaxAmount { get; set; }
    }
}