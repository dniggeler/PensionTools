namespace PensionCoach.Tools.CommonTypes
{
    public record TaxPersonBasic
    {
        public string Name { get; init; }
        public CivilStatus CivilStatus { get; init; }
        public int NumberOfChildren { get; init; }
        public ReligiousGroupType ReligiousGroupType { get; init; }
        public ReligiousGroupType? PartnerReligiousGroupType { get; init; }
    };
}
