using System.ComponentModel.DataAnnotations.Schema;


namespace Tax.Data.Abstractions.Models
{
    public class FederalTaxTariffModel
    {
        [Column("Steuer")]
        public decimal TaxAmount { get; set; }

        [Column("Einkommen")]
        public decimal IncomeLevel { get; set; }

        [Column("Jahr")]
        public int Year { get; set; }

        [Column("Steuerinkrement")]
        public decimal TaxIncrement { get; set; }

        [Column("Einkommensinkrement")]
        public decimal IncomeIncrement { get; set; }

        [Column("Tariftyp")]
        public int TariffType { get; set; }

        [Column("Steuertyp")]
        public int TaxType { get; set; }
    }
}