namespace TaxCalculator.WebApi.Models
{
    public class CapitalBenefitTaxComparerResponse
    {
        public string Name { get; set; }

        public int MunicipalityId { get; set; }

        public string MunicipalityName { get; set; }

        public int MaxSupportedTaxYear { get; set; }

        public decimal TotalTaxAmount { get; set; }

        public TaxAmountDetail TaxDetails { get; set; }
    }
}
