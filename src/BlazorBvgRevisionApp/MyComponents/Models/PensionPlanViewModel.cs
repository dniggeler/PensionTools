namespace BlazorBvgRevisionApp.MyComponents.Models;

public class PensionPlanViewModel
{
    public decimal InsuredSalary { get; set; }

    public decimal RetirementCapitalEndOfYear { get; set; }

    public decimal ProjectionInterestRate { get; set; }

    public decimal ConversionRate { get; set; }

    public RetirementCreditRange[] RetirementCredits { get; set; } = [];
}
