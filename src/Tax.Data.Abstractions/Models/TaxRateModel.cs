using System.ComponentModel.DataAnnotations.Schema;


namespace Tax.Data.Abstractions.Models
{
    public class TaxRateModel
    {
        [Column("Kanton")]
        public string Canton { get; set; }

        [Column("Jahr")]
        public int Year { get; set; }

        [Column("Gemeinde")]
        public string Municipality { get; set; }
        
        [Column("SteuerfussKanton")]
        public decimal TaxRateCanton { get; set; }

        [Column("SteuerfussGemeinde")]
        public decimal TaxRateMunicipality { get; set; }

        [Column("KircheKath")]
        public decimal RomanChurchTaxRate { get; set; }

        [Column("KircheRef")]
        public decimal ProtestantChurchTaxRate { get; set; }

        [Column("KircheChristKath")]
        public decimal CatholicChurchTaxRate { get; set; }
    }
}