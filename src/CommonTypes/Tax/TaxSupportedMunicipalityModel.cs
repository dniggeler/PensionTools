namespace PensionCoach.Tools.CommonTypes.Tax
{
    public record TaxSupportedMunicipalityModel
    {
        public int BfsMunicipalityNumber { get; set; }

        public string Name { get; set; }

        public Canton Canton { get; set; }
    }
}
