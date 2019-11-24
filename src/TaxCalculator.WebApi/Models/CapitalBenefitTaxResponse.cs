namespace TaxCalculator.WebApi.Models
{
    public class CapitalBenefitTaxResponse
    {
        public string  Name             { get; set; }
        public int     CalculationYear  { get; set; }
        public decimal CantonRate       { get; set; }
        public decimal MunicipalityRate { get; set; }
        public decimal TaxAmount        { get; set; }
        public decimal ChurchTaxAmount { get; set; }
    }
}