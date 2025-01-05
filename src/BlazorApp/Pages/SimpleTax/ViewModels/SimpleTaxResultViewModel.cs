namespace BlazorApp.Pages.SimpleTax.ViewModels;

public class SimpleTaxResultViewModel
{
    public string Name { get; set; }

    public int CalculationYear { get; set; }

    public string Municipality { get; set; }

    public decimal? TaxableAmount { get; set; }

    public decimal TotalTaxAmount { get; set; }

    public decimal CantonTaxAmount { get; set; }

    public decimal MunicipalityTaxAmount { get; set; }

    public decimal FederalTaxAmount { get; set; }

    public decimal ChurchTaxAmount { get; set; }

    public decimal PollTaxAmount { get; set; }

    public bool WithTaxBreakDown { get; set; }

    public decimal IncomeTaxAmount { get; set; }

    public decimal WealthTaxAmount { get; set; }
}
