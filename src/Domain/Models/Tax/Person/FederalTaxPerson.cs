namespace Domain.Models.Tax.Person
{
    public record FederalTaxPerson : TaxPersonBasic
    {
        public decimal TaxableAmount { get; set; } 
    }
}
