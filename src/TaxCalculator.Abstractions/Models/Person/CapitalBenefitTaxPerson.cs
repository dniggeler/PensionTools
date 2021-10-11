using PensionCoach.Tools.CommonTypes.Tax;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person
{
    public record CapitalBenefitTaxPerson : TaxPersonBasic
    {
        public decimal TaxableBenefits { get; init; } 
    }
}
