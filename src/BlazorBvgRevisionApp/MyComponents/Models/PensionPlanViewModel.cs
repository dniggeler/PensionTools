namespace BlazorBvgRevisionApp.MyComponents.Models;

public class PensionPlanViewModel
{
    public decimal? BvgInsuredSalary { get; set; }

    public decimal? BvgRetirementCapitalEndOfYear { get; set; }

    public decimal? InsuredSalary { get; set; }

    public decimal? RetirementCapitalEndOfYear { get; set; }

    public decimal ProjectionInterestRate { get; set; }

    public decimal ConversionRate { get; set; }

    public RetirementCreditRange[] RetirementCredits { get; set; } = [];
}
