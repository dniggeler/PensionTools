namespace PensionCoach.Tools.BvgCalculator
{
    public interface IBvgRetirementCredits
    {
        decimal GetRateInPercentage(int bvgAge);
        decimal GetRate(int bvgAge);
    }
}
