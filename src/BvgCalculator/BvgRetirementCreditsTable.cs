using System.Collections.Generic;
using static PensionCoach.Tools.CommonUtils.LevelValueDictionaryExtensions;

namespace PensionCoach.Tools.BvgCalculator
{
    public class BvgRetirementCreditsTable : IBvgRetirementCredits
    {
        public decimal GetRateInPercentage(int bvgAge)
        {
            return GetRate(bvgAge);
        }

        public decimal GetRate(int bvgAge)
        {
            return RetirementCreditDictionary.Match(bvgAge);
        }

        private static readonly Dictionary<int, decimal?> RetirementCreditDictionary =
            new Dictionary<int, decimal?>
            {
                {25, Bvg.RetirementCreditRateBvg.UpToAge24},
                {35, Bvg.RetirementCreditRateBvg.UpToAge34},
                {45, Bvg.RetirementCreditRateBvg.UpToAge44},
                {55, Bvg.RetirementCreditRateBvg.UpToAge54},
                {66, Bvg.RetirementCreditRateBvg.UpToAgeFinal},
                {9999, 0},
            };
    }
}
