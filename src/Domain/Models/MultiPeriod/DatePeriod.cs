using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Domain.Models.MultiPeriod;

/// <summary>
/// Represents a period of time between two dates.
/// Begin date cannot be later than end date.
/// Begin date is inclusive but end date is exclusive.
/// Begin date is meant to be the start of the day.
/// End date is meant to be the end of the day.
/// </summary>
/// Example: [2021-01-01, 2022-01-01[
[DebuggerDisplay("[{Begin}, {End}]")]
public readonly record struct DatePeriod : IEnumerable<DateOnly>
{
    public DateOnly Begin { get; }

    public DateOnly End { get; }

    public DatePeriod(DateOnly? begin = null, DateOnly? end = null)
    {
        Begin = begin ?? DateOnly.MinValue;
        End = end ?? DateOnly.MaxValue;

        if (Begin >= End)
        {
            throw new ArgumentException("Begin date cannot be later than end date.");
        }
    }

    public DatePeriod(DateOnly begin, int years)
    {
        Begin = begin;
        End = begin.AddYears(years);

        if (Begin >= End)
        {
            throw new ArgumentException("Begin date cannot be later than end date.");
        }
    }

    // Initialize the period from a string representation
    public DatePeriod(string period)
    {
        if (!IsValidFormat(period))
        {
            throw new ArgumentException("Invalid period format. Expected format: [yyyy-MM-dd, yyyy-MM-dd]");
        }

        string[] dates = period.Split(",");
        Begin = DateOnly.Parse(dates[0].Trim('[', ']', ' '));
        End = DateOnly.Parse(dates[1].Trim('[', ']' , ' '));
        if (Begin > End)
        {
            throw new ArgumentException("Begin date cannot be later than end date.");
        }
    }

    // checks if the given date is within the period
    public bool Contains(DateOnly date)
    {
        return Begin <= date && date < End;
    }

    // Initialize the period from a string representation. Format: "[yyyy-MM-dd, yyyy-MM-dd]"
    public static DatePeriod From(string periodAsString)
    {
        return new DatePeriod(periodAsString);
    }

    public bool Equals(DatePeriod other)
    {
        return Begin.Equals(other.Begin) && End.Equals(other.End);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Begin, End);
    }

    public override string ToString()
    {
        return $"[{Begin}, {End}]";
    }

    public IEnumerator<DateOnly> GetEnumerator()
    {
        for (DateOnly date = Begin; date < End; date = date.AddDays(1))
        {
            yield return date;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // Validate the format of the input string
    private static bool IsValidFormat(string period)
    {
        string pattern = @"^\[\d{4}-\d{2}-\d{2}, \d{4}-\d{2}-\d{2}\]$";
        return Regex.IsMatch(period, pattern);
    }
}
