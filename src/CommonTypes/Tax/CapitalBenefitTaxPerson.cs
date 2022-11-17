namespace PensionCoach.Tools.CommonTypes.Tax
{
    public record CapitalBenefitTaxPerson : TaxPersonBasic
    {
        public decimal TaxableCapitalBenefits { get; set; } 
    }
}
