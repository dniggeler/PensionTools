using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using PensionCoach.Tools.BvgCalculator.Models;
using PensionCoach.Tools.CommonUtils;

namespace PensionCoach.Tools.BvgCalculator
{
    /// <summary>
    /// 
    /// </summary>
    public static class BvgCapitalCalculationHelper
    {
        /// <summary>
        /// Gets the retirement capital sequence.
        /// </summary>
        /// <param name="processDate"></param>
        /// <param name="retirementDate">The date of retirement.</param>
        /// <param name="ageBvg">The age BVG.</param>
        /// <param name="retirementAgeBvg">The retirement age BVG.</param>
        /// <param name="iBvg">The i BVG.</param>
        /// <param name="predecessorCapital">The actuarial reserve accounting year.</param>
        /// <param name="retirementCreditSequence">The retirement credit sequence.</param>
        /// <returns></returns>
        public static IReadOnlyCollection<BvgRetirementCapital> GetRetirementCapitalSequence(
            DateTime processDate,
            DateTime retirementDate,
            int ageBvg,
            int retirementAgeBvg,
            decimal iBvg,
            PredecessorRetirementCapital predecessorCapital,
            IReadOnlyCollection<RetirementCredit> retirementCreditSequence)
        {
            // Begin of financial year = January 1 fo the financial year
            DateTime beginOfFinancialYear = new DateTime(processDate.Year, 1, 1);
            DateTime endOfFinancialYear = beginOfFinancialYear.AddYears(1);

            if (retirementDate <= endOfFinancialYear)
            {
                decimal aghBoYProRata = predecessorCapital.BeginOfYearAmount;
                decimal aghEoYProRata = predecessorCapital.EndOfYearAmount;

                BvgRetirementCapital aghProRataBoY =
                    new BvgRetirementCapital(beginOfFinancialYear,
                        aghBoYProRata,
                        aghBoYProRata);
                BvgRetirementCapital aghProRataEoY =
                    new BvgRetirementCapital(endOfFinancialYear,
                        aghEoYProRata,
                        aghEoYProRata);

                BvgRetirementCapital aghProRataEndOfPeriod =
                    aghProRataBoY
                        .Interpolate(true, retirementDate, aghProRataEoY)
                        .Round60()
                        .Round();

                return Prelude.List(aghProRataEndOfPeriod);
            }


            BvgRetirementCapital retirementCapitalEndOfYear =
                new BvgRetirementCapital(
                        endOfFinancialYear,
                        predecessorCapital.EndOfYearAmount,
                        predecessorCapital.EndOfYearAmount)
                    .Round();

            Lst<BvgRetirementCapital> retirementAssets = Prelude.List(retirementCapitalEndOfYear);

            return retirementAssets.AddRange(
                    GetProjection(
                        ageBvg + 1,
                        retirementDate,
                        endOfFinancialYear,
                        retirementCapitalEndOfYear,
                        iBvg,
                        retirementAgeBvg,
                        retirementCreditSequence))
                .Reverse();
        }

        /// <summary>
        /// Interpolates two values defined by date d1 and d2 with respect to begin and end dates.
        /// </summary>
        /// <param name="beginOfFullPeriod">The begin of full period.</param>
        /// <param name="d1">The d1.</param>
        /// <param name="d2">The d2.</param>
        /// <param name="endOfFullPeriod">The end of full period.</param>
        /// <param name="fullPeriodValue">The full period value.</param>
        /// <returns></returns>
        public static (decimal ValueD1, decimal ValueD2) InterpolateInterval(
            DateTime beginOfFullPeriod,
            DateTime d1,
            DateTime d2,
            DateTime endOfFullPeriod,
            decimal fullPeriodValue)
        {
            decimal decBeginOfFullPeriod = DateUtils.GetYears360(beginOfFullPeriod);
            decimal decEndOfFullPeriod = DateUtils.GetYears360(endOfFullPeriod);
            decimal decDate1 = DateUtils.GetYears360(d1);
            decimal decDate2 = DateUtils.GetYears360(d2);

            decimal endOfPeriodValue = MathUtils.Interpol(0, fullPeriodValue,
                (decDate2 - decBeginOfFullPeriod) / (decEndOfFullPeriod - decBeginOfFullPeriod));

            decimal beginOfPeriodValue = MathUtils.Interpol(0, fullPeriodValue,
                (decDate1 - decBeginOfFullPeriod) / (decEndOfFullPeriod - decBeginOfFullPeriod));

            return (beginOfPeriodValue, endOfPeriodValue);
        }

        /// <summary>
        /// Gets the retirement credit sequence.
        /// </summary>
        /// <param name="personDetails">The person details.</param>
        /// <param name="processDate">The process date.</param>
        /// <param name="salaryDetails"></param>
        /// <param name="plan"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException">Process date {processDate} after date of retirement {dateOfRetirement}</exception>
        internal static IReadOnlyCollection<RetirementCredit> GetRetirementCreditSequence(
            BvgPerson personDetails,
            DateTime processDate,
            BvgSalary salaryDetails)
        {
            int xsBvg = BvgCalculator.GetRetirementAge(personDetails.Gender);
            int xBvg = personDetails.DateOfBirth.GetBvgAge(processDate.Year);

            BvgRetirementCreditsTable bvgRetirementCreditTable = new BvgRetirementCreditsTable();

            return Enumerable.Range(xBvg, xsBvg - xBvg + 1)
                .Select(x =>
                    new RetirementCredit(bvgRetirementCreditTable.GetRate(x) * salaryDetails.InsuredSalary, x))
                .ToList();
        }

        private static List<BvgRetirementCapital> GetProjection(
            int age,
            DateTime dateOfRetirement,
            DateTime dateOfBeginOfPeriod,
            BvgRetirementCapital retirementCapitalBoY,
            decimal iProjection,
            int retirementAge,
            IReadOnlyCollection<RetirementCredit> retirementCreditSequence)
        {
            List<BvgRetirementCapital> assets = new List<BvgRetirementCapital>();

            BvgRetirementCapital newRetirementAssets = retirementCapitalBoY;

            for (int x = age; x < retirementAge; ++x)
            {
                RetirementCredit retirementCredits = retirementCreditSequence.FirstOrDefault(c => c.Age == x);

                newRetirementAssets = CalculateNewProjectedRetirementCapital(dateOfBeginOfPeriod.AddYears(x - age),
                    retirementCredits, newRetirementAssets.Value, newRetirementAssets.ValueWithoutInterest, iProjection);

                assets.Add(newRetirementAssets);
            }

            RetirementCredit retirementCreditsRetirementYear = retirementCreditSequence.FirstOrDefault(c => c.Age == retirementAge);

            BvgRetirementCapital oldRetirementAssets = newRetirementAssets;

            newRetirementAssets = CalculateNewProjectedRetirementCapital(dateOfBeginOfPeriod.AddYears(retirementAge - age),
                retirementCreditsRetirementYear, newRetirementAssets.Value, newRetirementAssets.ValueWithoutInterest, iProjection);

            assets.Add(oldRetirementAssets.Interpolate(true, dateOfRetirement, newRetirementAssets));

            return assets;
        }

        private static BvgRetirementCapital CalculateNewProjectedRetirementCapital(
            DateTime beginOfPeriod,
            RetirementCredit retirementCredit,
            decimal xCapital,
            decimal xCapitalWoI,
            decimal iProjection)
        {
            // Retirement assets calculated without interest BVG portion
            // WoI = without interest
            // Difference in without interest from without interest by plan:
            //   by plan takes the rounded retirement credits (to 0.1CHF) while
            //   the other calculates with effective credits
            decimal x1Capital = xCapital * (1M + iProjection) + retirementCredit.AmountRounded10;
            decimal x1CapitalWoI = xCapitalWoI + retirementCredit.AmountRaw;

            return new BvgRetirementCapital(
                beginOfPeriod.AddYears(1),
                x1Capital,
                x1CapitalWoI);
        }
    }
}
