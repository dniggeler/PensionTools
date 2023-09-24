namespace Domain.Models.Tax;

public record TaxPerson : TaxPersonBasic
{
    public decimal TaxableIncome { get; set; } 
    public decimal TaxableFederalIncome { get; set; } 
    public decimal TaxableWealth { get; set; }
}
