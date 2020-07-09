using System;
using PensionCoach.Tools.BvgCalculator.Models;
using PensionCoach.Tools.CommonUtils;

namespace PensionCoach.Tools.BvgCalculator
{
    public static class BvgCalculationExtensions
    {
        public static BvgRetirementCapital Round60(this BvgRetirementCapital element)
        {
            return RoundBy(element, MathUtils.Round60);
        }

        public static BvgRetirementCapital Round(this BvgRetirementCapital element)
        {
            return RoundBy(element, MathUtils.Round);
        }

        public static BvgRetirementCapital Interpolate(this BvgRetirementCapital element,
            bool isEndOfPeriod,
            DateTime date,
            BvgRetirementCapital other)
        {
            decimal interpol = DateUtils.YearsBetween(element.Date, date) / DateUtils.YearsBetween(element.Date, other.Date);

            return new BvgRetirementCapital(date,
                MathUtils.Interpol(element.Value, other.Value, interpol),
                MathUtils.Interpol(element.ValueWithoutInterest, other.ValueWithoutInterest, interpol));
        }

        private static BvgRetirementCapital RoundBy(BvgRetirementCapital element,
            Func<decimal, decimal> roundMethod)
        {
            return new BvgRetirementCapital(element.Date,
                roundMethod(element.Value),
                roundMethod(element.ValueWithoutInterest));
        }
    }
}
