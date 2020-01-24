using System.ComponentModel.DataAnnotations.Schema;


namespace Tax.Data.Abstractions.Models
{
    public class TaxRateEntity
    {
        [Column("BfsId")] 
        public int BfsId { get; set; }

        [Column("Jahr")]
        public int Year { get; set; }

        [Column("Kanton")]
        public string Canton { get; set; }

        [Column("Gemeindename")]
        public string MunicipalityName { get; set; }
        
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

        [Column("SteuerfussJuristPerson")]
        public decimal TaxRateCompany { get; set; }

        [Column("Personensteuer")]
        public decimal? PollTaxAmount { get; set; }
    }
}