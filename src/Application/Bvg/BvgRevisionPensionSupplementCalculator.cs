namespace Application.Bvg;

public class BvgRevisionPensionSupplementCalculator : IPensionSupplementCalculator
{
    public decimal CalculatePensionSupplement(
        DateTime dateOfBirth,
        decimal finalRetirementCapital)
    {
        const decimal lBound = 220500;
        const decimal uBound = lBound * 2M;

        const decimal pensionLowerBound = 1200;
        const decimal pensionMediumBound = pensionLowerBound * 1.5M;
        const decimal pensionUpperBound = pensionLowerBound * 2M;

        decimal s = decimal.One - (finalRetirementCapital - lBound) / (uBound - lBound);

        decimal pensionIncrease = (dateOfBirth.Year, finalRetirementCapital) switch
        {
            (_, > uBound) => decimal.Zero,
            ( < 1961, _) => decimal.Zero,
            ( >= 1961 and <= 1965, <= lBound) => pensionUpperBound,
            ( <= 1970, <= lBound) => pensionMediumBound,
            ( <= 1975, <= lBound) => pensionLowerBound,
            ( >= 1961 and <= 1965, < uBound) => pensionUpperBound * s,
            ( <= 1970, < uBound) => pensionMediumBound * s,
            ( <= 1975, < uBound) => pensionLowerBound * s,
            _ => decimal.Zero
        };

        return pensionIncrease;
    }
}
