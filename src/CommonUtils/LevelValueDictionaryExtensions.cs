using System;
using System.Collections.Generic;

namespace PensionCoach.Tools.CommonUtils
{
    /// <summary>
    /// Extension methods related to date calculations in
    /// the context of occupational benefits (Vorsorge)
    /// </summary>
    public static class LevelValueDictionaryExtensions
    {
        public static decimal Match(this Dictionary<int, decimal?> dictionary, int level, string errorMessage = null)
        {
            return MatchLevel(dictionary, level, false, errorMessage ?? nameof(errorMessage));
        }

        public static decimal Match(this Dictionary<int, decimal?> dictionary, int level, bool isLessOrEqual, string errorMessage = null)
        {
            return MatchLevel(dictionary, level, isLessOrEqual, errorMessage ?? nameof(errorMessage));
        }

        private static decimal MatchLevel(Dictionary<int, decimal?> dictionary, int level, bool isLessOrEqual, string errorMsg)
        {
            static bool LessThan(int v1, int v2) => v1 < v2;
            static bool LessThanOrEqual(int v1, int v2) => v1 <= v2;

            Func<int, int, bool> comparer = isLessOrEqual ? LessThanOrEqual : (Func<int, int, bool>)LessThan;

            foreach (KeyValuePair<int, decimal?> kvp in dictionary)
            {
                if (comparer(level, kvp.Key))
                {
                    if (kvp.Value.HasValue)
                    {
                        return kvp.Value.Value;
                    }

                    throw new ArgumentException(errorMsg, nameof(level));
                }
            }

            return default;
        }
    }
}
