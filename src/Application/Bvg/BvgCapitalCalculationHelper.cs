using Application.Extensions;
using Domain.Models.Bvg;
using PensionCoach.Tools.CommonUtils;
using static LanguageExt.Prelude;

namespace Application.Bvg
{
    /// <summary>
    /// 
    /// </summary>
    public static class BvgCapitalCalculationHelper
    {
        /// <summary>
        /// Gets the retirement capital sequence.
        /// </summary>
        /// <param name="calculationYear"></param>
        /// <param name="retirementDate">The date of retirement.</param>
        /// <param name="ageBvg">The age BVG.</param>
        /// <param name="retirementAgeBvg">The retirement age BVG.</param>
        /// <param name="iBvg">The i BVG.</param>
        /// <param name="retirementCapitalEndOfYear">The actuarial reserve accounting year.</param>
        /// <param name="retirementCreditSequence">The retirement credit sequence.</param>
        /// <returns></returns>
        public static IReadOnlyCollection<RetirementCapital> GetRetirementCapitalSequence(
            int calculationYear,
            DateTime retirementDate,
            int ageBvg,
            int retirementAgeBvg,
            decimal iBvg,
            decimal retirementCapitalEndOfYear,
            IReadOnlyCollection<RetirementCredit> retirementCreditSequence)
        {
            // Begin of financial year = January 1 fo the financial year
            DateTime beginOfFinancialYear = new DateTime(calculationYear, 1, 1);
            DateTime endOfFinancialYear = beginOfFinancialYear.AddYears(1);

            if (retirementDate <= endOfFinancialYear)
            {
                return [new RetirementCapital(retirementDate,
                    retirementCapitalEndOfYear,
                    retirementCapitalEndOfYear)];
            }

            RetirementCapital retirementCapitalItem = new (endOfFinancialYear, retirementCapitalEndOfYear, retirementCapitalEndOfYear);

            var retirementAssets = List(retirementCapitalItem);

            retirementAssets = retirementAssets.AddRange(
                GetProjection(
                    ageBvg + 1,
                    retirementDate,
                    endOfFinancialYear,
                    retirementCapitalItem,
                    iBvg,
                    retirementAgeBvg,
                    retirementCreditSequence));

            return retirementAssets.Reverse();
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

        private static List<RetirementCapital> GetProjection(
            int age,
            DateTime dateOfRetirement,
            DateTime dateOfBeginOfPeriod,
            RetirementCapital retirementCapitalBoY,
            decimal iProjection,
            int retirementAge,
            IReadOnlyCollection<RetirementCredit> retirementCreditSequence)
        {
            List<RetirementCapital> assets = new List<RetirementCapital>();

            RetirementCapital newRetirementAssets = retirementCapitalBoY;

            for (int x = age; x < retirementAge; ++x)
            {
                RetirementCredit retirementCredits = retirementCreditSequence.FirstOrDefault(c => c.Age == x);

                newRetirementAssets = CalculateNewProjectedRetirementCapital(dateOfBeginOfPeriod.AddYears(x - age),
                    retirementCredits, newRetirementAssets.Value, newRetirementAssets.ValueWithoutInterest, iProjection);

                assets.Add(newRetirementAssets);
            }

            RetirementCredit retirementCreditsRetirementYear = retirementCreditSequence.FirstOrDefault(c => c.Age == retirementAge);

            RetirementCapital oldRetirementAssets = newRetirementAssets;

            newRetirementAssets = CalculateNewProjectedRetirementCapital(dateOfBeginOfPeriod.AddYears(retirementAge - age),
                retirementCreditsRetirementYear, newRetirementAssets.Value, newRetirementAssets.ValueWithoutInterest, iProjection);

            assets.Add(oldRetirementAssets.Interpolate(true, dateOfRetirement, newRetirementAssets));

            return assets;
        }

        private static RetirementCapital CalculateNewProjectedRetirementCapital(
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
            decimal x1Capital = xCapital * (1M + iProjection) + MathUtils.Round10(retirementCredit.AmountRaw);
            decimal x1CapitalWoI = xCapitalWoI + retirementCredit.AmountRaw;

            return new RetirementCapital(
                beginOfPeriod.AddYears(1),
                x1Capital,
                x1CapitalWoI);
        }
    }
}
