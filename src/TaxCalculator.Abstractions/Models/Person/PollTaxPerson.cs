using LanguageExt;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person
{
    public class PollTaxPerson
    {
        public string Name { get; set; }
        public string Municipality { get; set; }
        public string Canton { get; set; }
        public Option<CivilStatus> CivilStatus { get; set; }
    }
}