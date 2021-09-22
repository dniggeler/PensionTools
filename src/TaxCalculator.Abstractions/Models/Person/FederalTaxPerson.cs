using PensionCoach.Tools.CommonTypes;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person
{
    public record FederalTaxPerson : TaxPersonBasic
    {
        public decimal TaxableAmount { get; set; } 
    }
}
