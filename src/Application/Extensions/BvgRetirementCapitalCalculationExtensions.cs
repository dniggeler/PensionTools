using Domain.Models.Bvg;
using PensionCoach.Tools.CommonUtils;

namespace Application.Extensions;

public static class BvgCalculationExtensions
{
    public static RetirementCapital Round60(this RetirementCapital element)
    {
        return RoundBy(element, MathUtils.Round60);
    }

    public static RetirementCapital Round(this RetirementCapital element)
    {
        return RoundBy(element, MathUtils.Round);
    }

    public static RetirementCapital Interpolate(this RetirementCapital element,
        bool isEndOfPeriod,
        DateTime date,
        RetirementCapital other)
    {
        decimal interpol = DateUtils.YearsBetween(element.Date, date) / DateUtils.YearsBetween(element.Date, other.Date);

        return new RetirementCapital(date,
            MathUtils.Interpol(element.Value, other.Value, interpol),
            MathUtils.Interpol(element.ValueWithoutInterest, other.ValueWithoutInterest, interpol));
    }

    private static RetirementCapital RoundBy(RetirementCapital element,
        Func<decimal, decimal> roundMethod)
    {
        return new RetirementCapital(element.Date,
            roundMethod(element.Value),
            roundMethod(element.ValueWithoutInterest));
    }
}
