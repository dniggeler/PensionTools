namespace BlazorApp.ViewModels;

public class InvestmentDefinitionViewModel
{
    public int Year { get; set; }
        
    public int NumberOfPeriods { get; set; }

    public decimal InitialAmount { get; set; }

    public decimal RecurrentAmount { get; set; }
}
