using LanguageExt;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person
{
    public class ChurchTaxPerson
    {
        public string Name { get; set; }
        public string Municipality { get; set; }
        public Canton Canton { get; set; }
        public Option<CivilStatus> CivilStatus { get; set; }
        public Option<ReligiousGroupType> ReligiousGroup { get; set; }
        public Option<ReligiousGroupType> PartnerReligiousGroup { get; set; }
    }
}