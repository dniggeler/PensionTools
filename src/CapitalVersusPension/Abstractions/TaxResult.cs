namespace CapitalVersusPension.Abstractions
{
    public class TaxResult
    {
        public decimal FederalTaxAmount { get; set; }

        public decimal CantonTaxAmount { get; set; }

        public decimal MunicipalTaxAmount { get; set; }

        public decimal TaxRate { get; set; }
    }
}