using LanguageExt;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person
{
    public class CapitalBenefitTaxPerson
    {
        public string Name { get; set; }
        public string Municipality { get; set; }
        public string Canton { get; set; }
        public Option<CivilStatus> CivilStatus { get; set; }
        public decimal TaxableIncome { get; set; } 
        public decimal TaxableFederalIncome { get; set; } 
        public decimal TaxableWealth { get; set; }
        public int NumberOfChildren { get; set; }
        public Option<ReligiousGroupType> ReligiousGroupType { get; set; }
        public Option<ReligiousGroupType> PartnerReligiousGroupType { get; set; }
    }
}