namespace BlazorBvgRevisionApp.MyComponents.Models;

public class EligibleForPensionViewModel
{
    public bool IsEligibleAhv { get; set; }
    public bool IsEligibleBvg { get; set; }

    public bool IsEligible() => IsEligibleBvg && IsEligibleAhv;
}
