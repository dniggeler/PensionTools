using System;

namespace PensionCoach.Tools.CommonTypes
{
    public class MunicipalityModel
    {
        public int BfsNumber { get; set; }

        public string Name { get; set; }

        public Canton Canton { get; set; }

        public DateTime? DateOfMutation { get; set; }

        public int MutationId { get; set; }

        public int? SuccessorId { get; set; }
    }
}
