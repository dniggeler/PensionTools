using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person
{
    public record BasisTaxPerson : TaxPersonBasic
    {
        public decimal TaxableAmount { get; set; } 
    }
}
