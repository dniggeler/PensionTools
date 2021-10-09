namespace PensionCoach.Tools.CommonTypes.Tax
{
    public record TaxPersonBasic
    {
        public string Name { get; set; }
        public CivilStatus CivilStatus { get; set; }
        public int NumberOfChildren { get; set; }
        public ReligiousGroupType ReligiousGroupType { get; set; }
        public ReligiousGroupType? PartnerReligiousGroupType { get; set; }
    };
}
