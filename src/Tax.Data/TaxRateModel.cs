using System.ComponentModel.DataAnnotations.Schema;


namespace Tax.Data
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
    }
}