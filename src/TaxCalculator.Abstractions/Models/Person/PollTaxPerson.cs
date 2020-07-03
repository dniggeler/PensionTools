using LanguageExt;
using PensionCoach.Tools.CommonTypes;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person
{
    public class PollTaxPerson
    {
        public string Name { get; set; }

        public Option<CivilStatus> CivilStatus { get; set; }
    }
}