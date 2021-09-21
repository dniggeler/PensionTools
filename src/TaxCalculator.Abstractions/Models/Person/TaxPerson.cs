using LanguageExt;
using PensionCoach.Tools.CommonTypes;


namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person
{
    public class TaxPerson
    {
        public string Name { get; set; }
        public Option<CivilStatus> CivilStatus { get; set; }
        public decimal TaxableIncome { get; set; } 
        public decimal TaxableFederalIncome { get; set; } 
        public decimal TaxableWealth { get; set; }
        public int NumberOfChildren { get; set; }
        public ReligiousGroupType ReligiousGroupType { get; set; }
        public Option<ReligiousGroupType> PartnerReligiousGroupType { get; set; }
    }
}
