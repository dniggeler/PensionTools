using Domain.Models.MultiPeriod;

namespace Domain.Tests;

[Trait("Domain Tests", "Date Period")]
public class DatePeriodTest
{
    [Theory(DisplayName = "==, !=, Equals")]
    [MemberData(nameof(GetDatePeriodData))]
    public void EqualTest_IsValid(DateOnly begin1, DateOnly end1, DateOnly begin2, DateOnly end2)
    {
        DatePeriod period1 = new(begin1, end1);
        DatePeriod period2 = new(begin2, end2);

        Assert.True(period1 == period2);
        Assert.True(period1.Equals(period2));
        Assert.False(period1 != period2);
    }

    [Theory(DisplayName = "Contains Date")]
    [MemberData(nameof(GetDateContainsData))]
    public void ContainsTest_IsValid(DateOnly begin, DateOnly end, DateOnly date, bool expected)
    {
        DatePeriod period = new(begin, end);
        Assert.Equal(expected, period.Contains(date));
    }

    [Theory(DisplayName = "Enumerator")]
    [MemberData(nameof(GetDatePeriodEnumeratorData))]
    public void EnumeratorTest_IsValid(DateOnly begin, DateOnly end, List<DateOnly> expectedDates)
    {
        DatePeriod period = new(begin, end);
        List<DateOnly> actualDates = new();

        foreach (var date in period)
        {
            actualDates.Add(date);
        }

        Assert.Equal(expectedDates, actualDates);
    }

    [Theory(DisplayName = "From String")]
    [MemberData(nameof(GetDatePeriodFromStringData))]
    public void FromStringTest_IsValid(string period, DateOnly expectedBegin, DateOnly expectedEnd)
    {
        DatePeriod datePeriod = DatePeriod.From(period);
        DatePeriod expectedPeriod = new(expectedBegin, expectedEnd);

        Assert.Equal(expectedPeriod, datePeriod);
    }


    public static IEnumerable<object[]> GetDatePeriodFromStringData()
    {
        yield return
        [
            "[2023-01-01, 2024-01-01]",
            new DateOnly(2023, 1, 1),
            new DateOnly(2024, 1, 1)
        ];

        yield return
        [
            "[2023-06-01, 2023-07-01]",
            new DateOnly(2023, 6, 1),
            new DateOnly(2023, 7, 1)
        ];

        yield return
        [
            "[2023-01-01, 2023-01-02]",
            new DateOnly(2023, 1, 1),
            new DateOnly(2023, 1, 2)
        ];
    }

    public static IEnumerable<object[]> GetDatePeriodData()
    {
        yield return
        [
            new DateOnly(2023, 1, 1),
            new DateOnly(2024, 1, 1),
            new DateOnly(2023, 1, 1),
            new DateOnly(2024, 1, 1)
        ];

        yield return
        [
            new DateOnly(2023, 6, 1),
            new DateOnly(2023, 7, 1),
            new DateOnly(2023, 6, 1),
            new DateOnly(2023, 7, 1)
        ];

        yield return
        [
            new DateOnly(2023, 1, 1),
            new DateOnly(2023, 1, 2),
            new DateOnly(2023, 1, 1),
            new DateOnly(2023, 1, 2)
        ];
    }

    public static IEnumerable<object[]> GetDateContainsData()
    {
        yield return
        [
            new DateOnly(2023, 1, 1),
            new DateOnly(2023, 12, 31),
            new DateOnly(2023, 6, 15),
            true
        ];

        yield return
        [
            new DateOnly(2023, 1, 1),
            new DateOnly(2024, 1, 1),
            new DateOnly(2024, 1, 1),
            false
        ];

        yield return
        [
            new DateOnly(2023, 6, 1),
            new DateOnly(2023, 6, 30),
            new DateOnly(2023, 6, 1),
            true
        ];

        yield return
        [
            new DateOnly(2023, 6, 1),
            new DateOnly(2023, 6, 30),
            new DateOnly(2023, 5, 31),
            false
        ];

        yield return
        [
            new DateOnly(2023, 6, 1),
            new DateOnly(2023, 6, 2),
            new DateOnly(2023, 6, 1),
            true
        ];
    }

    public static IEnumerable<object[]> GetDatePeriodEnumeratorData()
    {
        yield return
        [
            new DateOnly(2023, 1, 1),
            new DateOnly(2023, 1, 3),
            new List<DateOnly>
            {
                new(2023, 1, 1),
                new(2023, 1, 2),
            }
        ];

        yield return
        [
            new DateOnly(2023, 6, 1),
            new DateOnly(2023, 6, 3),
            new List<DateOnly>
            {
                new(2023, 6, 1),
                new(2023, 6, 2),
            }
        ];

        yield return
        [
            new DateOnly(2023, 12, 30),
            new DateOnly(2024, 1, 1),
            new List<DateOnly>
            {
                new(2023, 12, 30),
                new(2023, 12, 31),
            }
        ];
    }
}
