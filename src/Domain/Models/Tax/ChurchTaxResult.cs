namespace Domain.Models.Tax;

public class ChurchTaxResult
{
    public decimal? TaxAmount { get; set; }

    public decimal? TaxAmountPartner { get; set; }

    public decimal TaxRate { get; set; }
}
