using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tax.Data.Abstractions.Models
{
    public class MunicipalityEntity
    {
        [Column("BfsId")]
        public int BfsNumber { get; set; }

        [Column("Gemeindename")]
        public string Name { get; set; }

        [Column("Kanton")]
        public string Canton { get; set; }

        [Column("DateOfMutation")]
        public string DateOfMutation { get; set; }

        [Column("MutationId")]
        public int MutationId { get; set; }

        [Column("SuccessorId")]
        public int? SuccessorId { get; set; }
    }
}