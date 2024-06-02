using System.Diagnostics;

namespace Domain.Models.Bvg;

/// <summary>
/// It is defined as technical age. This means years and months
/// derived from the technical date of birth and a given process date expressed as (years, months).
/// </summary>
/// <remarks>
/// Example: Vorfalldatum: 22.1.2021, Geburtsdatum: 15.1.1958
/// => techn. Geburtsdatum 1.2.1958
/// => techn. Alter = 2021.1 - 1958.2 = 62.1
/// </remarks>
[DebuggerDisplay("{Years}/{Months}")]
public record TechnicalAge(int Years, int Months)
{
    private const int NumMonths = 12;

    public static TechnicalAge operator +(TechnicalAge a, TechnicalAge b)
    {
        int totalMonths = a.Months + b.Months;
        int extraYears = totalMonths / NumMonths;
        int remainingMonths = totalMonths % NumMonths;

        return new TechnicalAge(a.Years + b.Years + extraYears, remainingMonths);
    }

    public static TechnicalAge operator -(TechnicalAge a, TechnicalAge b)
    {
        int diffYears = a.Years - b.Years;
        int diffMonths = a.Months - b.Months;

        if (diffMonths >= 0)
        {
            return new TechnicalAge(diffYears, diffMonths);
        }

        diffYears--;
        diffMonths += NumMonths;

        return new TechnicalAge(diffYears, diffMonths);
    }

    public static bool operator <(TechnicalAge a, TechnicalAge b)
    {
        if (a.Years < b.Years)
        {
            return true;
        }

        if (a.Years == b.Years)
        {
            return a.Months < b.Months;
        }

        return false;
    }

    public static bool operator >(TechnicalAge a, TechnicalAge b)
    {
        return !(a <= b);
    }

    public static bool operator <=(TechnicalAge a, TechnicalAge b)
    {
        if (a.Years < b.Years)
        {
            return true;
        }

        if (a.Years == b.Years)
        {
            return a.Months <= b.Months;
        }

        return false;
    }

    public static bool operator >=(TechnicalAge a, TechnicalAge b)
    {
        return !(a < b);
    }

    public static implicit operator TechnicalAge((int Years, int Months) age)
    {
        return new TechnicalAge(age.Years, age.Months);
    }

    public static TechnicalAge From(int years, int months)
    {
        return new TechnicalAge(years, months);
    }
}
