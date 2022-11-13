using System;

namespace PensionCoach.Tools.CommonUtils
{
    public static class DateUtilsExtensions
    {
        /// <summary>
        /// Gets the years360.
        /// Example: 2015,1,1 -> 2015.0
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static decimal GetYears360(this DateTime d)
        {
            return DateUtils.GetDays360(d) / DateUtils.NumDaysOfYearAsDecimal;
        }

        public static DateTime GetDate(this DateTime dateOfBirth, int bvgAge)
        {
            return new DateTime(dateOfBirth.Year + bvgAge + 1, 1, 1);
        }

        public static DateTime BeginOfYearDate(this DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }

        public static DateTime GetEndOfYearDate(this DateTime date)
        {
            return new DateTime(date.Year, 1, 1).AddYears(1);
        }
    }
}
