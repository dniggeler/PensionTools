namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    public class TaxSupportedMunicipalityModel
    {
        public int BfsNumber { get; set; }

        public string Name { get; set; }

        public Canton Canton { get; set; }
    }
}