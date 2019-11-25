using LanguageExt;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person
{
    public class FederalCapitalBenefitTaxPerson
    {
        public string Name { get; set; }
        public Option<CivilStatus> CivilStatus { get; set; }
        public decimal TaxableBenefits { get; set; } 
    }
}