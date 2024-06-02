using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Tax
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
        public decimal IncomeIncrement { get; set; } = 100;

        [Column("Tariftyp")]
        public int TariffType { get; set; }
    }
}
