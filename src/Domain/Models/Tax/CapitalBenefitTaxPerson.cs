namespace Domain.Models.Tax;

public record CapitalBenefitTaxPerson : TaxPersonBasic
{
    public decimal TaxableCapitalBenefits { get; set; } 
}
