namespace TaxCalculator.WebApi.Models
{
    public class CapitalBenefitTaxResponse
    {
        public string Name { get; set; }

        public int CalculationYear { get; set; }

        public decimal TotalTaxAmount { get; set; }

        public TaxDetailResponse TaxDetails { get; set; }
    }

    public class TaxDetailResponse
    {
        public decimal FederalTaxAmount { get; set; }

        public decimal MunicipalityTaxAmount { get; set; }

        public decimal StateTaxAmount { get; set; }

        public decimal ChurchTaxAmount { get; set; }
    }
}