namespace TaxCalculator.WebApi.Models
{
    public class CapitalBenefitTaxResponse
    {
        public string Name { get; set; }

        public int CalculationYear { get; set; }

        public decimal TotalTaxAmount { get; set; }

        public TaxDetailResponse TaxDetails { get; set; }
    }
}