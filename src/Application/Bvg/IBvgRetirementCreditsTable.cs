namespace Application.Bvg;

public interface IBvgRetirementCredits
{
    decimal GetRateInPercentage(int bvgAge);

    decimal GetRate(int bvgAge);
}
