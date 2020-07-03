using LanguageExt;
using PensionCoach.Tools.CommonTypes;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person
{
    public class CapitalBenefitTaxPerson
    {
        public string Name { get; set; }
        public Option<CivilStatus> CivilStatus { get; set; }
        public decimal TaxableBenefits { get; set; } 
        public int NumberOfChildren { get; set; }
        public Option<ReligiousGroupType> ReligiousGroupType { get; set; }
        public Option<ReligiousGroupType> PartnerReligiousGroupType { get; set; }
    }
}