using System;

namespace PensionCoach.Tools.CommonUtils
{
    /// <summary>
    /// 
    /// </summary>
    public static class DateUtils
    {
        /// <summary>
        /// Number of days of a year
        /// </summary>
        public static readonly decimal NumDaysOfYearAsDecimal = 360M;
        public static readonly int NumDaysOfYear = 360;

        /// <summary>
        /// Number of days of each month
        /// </summary>
        public static readonly decimal NumDaysOfMonthAsDecimal = 30M;
        public static readonly int NumDaysOfMonth = 30;

        /// <summary>
        /// Number of days between two dates after German method
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static decimal DaysBetween(DateTime from, DateTime to)
        {
            DateTime from1 = from.AddDays(1.0);
            DateTime to1 = to.AddDays(1.0);

            int a = from1.Year * 360 + from1.Month * 30 + from1.Day;
            int b = to1.Year * 360 + to1.Month * 30 + to1.Day;

            return b - a;
        }

        /// <summary>
        /// Years and fraction between two dates after German method
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static decimal YearsBetween(DateTime from, DateTime to)
        {
            return DaysBetween(from, to) / NumDaysOfYear;
        }

        /// <summary>
        /// Number of days between two dates after German method
        /// </summary>
        /// <param name="fromYearJanuary1"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static decimal DaysBetween(int fromYearJanuary1, DateTime to)
        {
            DateTime to1 = to.AddDays(1.0);

            int a = fromYearJanuary1 * NumDaysOfYear + NumDaysOfMonth + 1 + 1; // y * 360 + january * 30 + first + shifts
            int b = to1.Year * NumDaysOfYear + to1.Month * NumDaysOfMonth + to1.Day;

            return b - a;
        }

        /// <summary>
        /// Years and fraction between two dates after German method
        /// </summary>
        /// <param name="fromYearJanuary1"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static decimal YearsBetween(int fromYearJanuary1, DateTime to)
        {
            return DaysBetween(fromYearJanuary1, to) / NumDaysOfYear;
        }

        /// <summary>
        /// Fractions the of year.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static decimal FractionOfYear(DateTime d)
        {
            DateTime d1 = d.AddDays(1.0); // shift +1

            int dayOfYear = (d1.Month - 1) * NumDaysOfMonth + (d1.Day - 1);

            // yyyy-12-31 (-> [yyyy+1]-01-01)
            if (dayOfYear == 0)
            {
                dayOfYear = NumDaysOfYear;
            }

            return (dayOfYear - 1) / NumDaysOfYearAsDecimal;
        }

        /// <summary>
        /// Number of days between two dates after German method
        /// </summary>
        /// <param name="from"></param>
        /// <param name="toYearJanuary1"></param>
        /// <returns></returns>
        public static decimal DaysBetween(DateTime from, int toYearJanuary1)
        {
            DateTime from1 = from.AddDays(1.0);

            int a = from1.Year * NumDaysOfYear + from1.Month * NumDaysOfMonth + from1.Day;
            int b = toYearJanuary1 * NumDaysOfYear + NumDaysOfMonth + 1 + 1; // y * 360 + january * 30 + first + shift

            return b - a;
        }

        /// <summary>
        /// Years and fraction between two dates after German method
        /// </summary>
        /// <param name="from"></param>
        /// <param name="toYearJanuary1"></param>
        /// <returns></returns>
        public static decimal YearsBetween(DateTime from, int toYearJanuary1)
        {
            return DaysBetween(from, toYearJanuary1) / NumDaysOfYear;
        }

        /// <summary>
        /// Gets the days360.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static int GetDays360(DateTime d)
        {
            DateTime d1 = d
                .AddDays(1.0); // shift +1
            return d1.Year * NumDaysOfYear
                   + (d1.Month - 1) * NumDaysOfMonth
                   + (d1.Day - 1)
                   - 1;           // shift -1
        }

        /// <summary>
        /// Gets the years360.
        /// Example: 2015,1,1 -> 2015.0
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static decimal GetYears360(DateTime d)
        {
            return GetDays360(d) / NumDaysOfYearAsDecimal;
        }

        public static (int Years, int Months) GetTechnicalAge(DateTime birthday, DateTime currentDate)
        {
            const int numMonth = 12;

            DateTime dateOfBirthTechnical = new DateTime(birthday.Year, birthday.Month, 1).AddMonths(1);
            DateTime currentDateTechnical = currentDate.AddDays(1);

            int years = currentDateTechnical.Year - dateOfBirthTechnical.Year;
            int months = currentDateTechnical.Month - dateOfBirthTechnical.Month;

            if (months < 0)
            {
                years--;
                months = numMonth + currentDateTechnical.Month - dateOfBirthTechnical.Month;
            }

            return (years, months);
        }
    }
}
