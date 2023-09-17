namespace Infrastructure.EstvTaxCalculator.Client.Models;

public class TaxLocationRequest
{
    public string Search { get; set; }

    public int Language { get; set; } = 1;
}
