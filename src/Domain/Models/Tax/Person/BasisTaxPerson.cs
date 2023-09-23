namespace Domain.Models.Tax.Person;

public record BasisTaxPerson : TaxPersonBasic
{
    public decimal TaxableAmount { get; set; } 
}
