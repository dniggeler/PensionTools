using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Municipality
{
    public class MunicipalityEntity
    {
        [Column("BfsId")]
        public int BfsNumber { get; set; }

        [Column("Gemeindename")]
        public string Name { get; set; }

        [Column("GemeindenameClean")]
        public string CleanName { get; set; }

        [Column("GemeindenameOverruled")]
        public string OverruledName { get; set; }

        [Column("Kanton")]
        public string Canton { get; set; }

        [Column("DateOfMutation")]
        public string DateOfMutation { get; set; }

        [Column("MutationId")]
        public int MutationId { get; set; }


        [Column("MutationsNummer")]
        public int MutationType { get; set; }

        [Column("SuccessorId")]
        public int? SuccessorId { get; set; }

        [Column("TaxLocationId")]
        public int? TaxLocationId { get; set; }

        [Column("Plz")]
        public string ZipCode { get; set; }

        [Column("Remark")]
        public string Remark { get; set; }
    }
}
