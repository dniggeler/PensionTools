using PensionCoach.Tools.CommonTypes;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    public record TaxSupportedMunicipalityModel
    {
        public int BfsNumber { get; init; }

        public string Name { get; init; }

        public Canton Canton { get; init; }
    }
}
