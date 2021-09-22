using PensionCoach.Tools.CommonTypes;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person
{
    public record TaxPerson : TaxPersonBasic
    {
        public decimal TaxableIncome { get; set; } 
        public decimal TaxableFederalIncome { get; set; } 
        public decimal TaxableWealth { get; set; }
    }
}
