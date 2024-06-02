namespace Application.Bvg.Models;

/// <summary>
/// Represents a date in the BVG context able to distinguish begin and end of day date (time 00:00 vs 24:00).
/// eg. 17.09.2021 00:00 vs 17.09.2021 24:00 but one is the begin and the other the end of the day.
/// </summary>
/// <param name="DateTime"></param>
/// <param name="IsEndOfDay"></param>
public readonly record struct BvgDate(DateTime DateTime, bool IsEndOfDay)
{
    // construct a BvgDate from a DateTime
    public BvgDate(DateOnly date, bool isEndOfDay=false) : this(date.ToDateTime(TimeOnly.MinValue), isEndOfDay)
    {}

    public static bool operator <(BvgDate left, BvgDate right) => left.DateTime < right.DateTime || (left.DateTime == right.DateTime && !left.IsEndOfDay && right.IsEndOfDay);

    public static bool operator >(BvgDate left, BvgDate right) => left.DateTime > right.DateTime || (left.DateTime == right.DateTime && left.IsEndOfDay && !right.IsEndOfDay);

    public static bool operator <=(BvgDate left, BvgDate right) => !(left > right);

    public static bool operator >=(BvgDate left, BvgDate right) => !(left < right);

    public static implicit operator BvgDate(DateTime dateTime)
    {
        return new BvgDate(dateTime, false);
    }

    public DateTime ToDateTime() => IsEndOfDay ? DateTime.AddDays(1) : DateTime;
}
