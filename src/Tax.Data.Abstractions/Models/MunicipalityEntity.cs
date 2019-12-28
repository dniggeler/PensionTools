using System.ComponentModel.DataAnnotations.Schema;

namespace Tax.Data.Abstractions.Models
{
    public class MunicipalityEntity
    {
        [Column("BfsId")]
        public int BfsNumber { get; set; }

        [Column("Gemeindename")]

        public string Name { get; set; }


        [Column("Bezirksname")]

        public string District { get; set; }

        [Column("Kanton")]

        public string Canton { get; set; }
    }
}