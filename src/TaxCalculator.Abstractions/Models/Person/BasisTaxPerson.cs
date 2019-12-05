using LanguageExt;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person
{
    public class BasisTaxPerson
    {
        public string Name { get; set; }
        public Canton Canton { get; set; }
        public Option<CivilStatus> CivilStatus { get; set; }
        public decimal TaxableAmount { get; set; } 
    }
}