using Domain.Enums;

namespace Domain.Models.Municipality
{
    public class MunicipalityModel
    {
        public int BfsNumber { get; set; }

        public string Name { get; set; }

        public Canton Canton { get; set; }

        public DateTime? DateOfMutation { get; set; }

        public int MutationId { get; set; }

        public int? SuccessorId { get; set; }

        public int? EstvTaxLocationId { get; set; }
    }
}
