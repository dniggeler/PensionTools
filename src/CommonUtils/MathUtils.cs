using System;

namespace PensionCoach.Tools.CommonUtils
{
    /// <summary>
    /// 
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Pows the specified base.
        /// </summary>
        /// <param name="base">The base.</param>
        /// <param name="exponent">The exponent.</param>
        /// <returns></returns>
        public static decimal Pow(decimal @base, int exponent)
        {
            return @base.Pow(exponent);
        }

        /// <summary>
        /// Rounds the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static decimal Round(decimal d, decimal precision)
        {
            return Math.Round(d / precision, 0, MidpointRounding.AwayFromZero) * precision;
        }

        /// <summary>
        /// Rounds the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static decimal? Round(decimal? d, decimal precision)
        {
            if (!d.HasValue)
            {
                return null;
            }

            return Math.Round(d.Value / precision, 0, MidpointRounding.AwayFromZero) * precision;
        }

        /// <summary>
        /// Rounds the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static decimal Round(decimal d)
        {
            return Round(d, 1M);
        }

        /// <summary>
        /// Rounds the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static decimal? Round(decimal? d)
        {
            return Round(d, 1M);
        }

        /// <summary>
        /// Round5s the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static decimal Round5(decimal d)
        {
            return Round(d, Precision005);
        }

        /// <summary>
        /// Round5s the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static decimal? Round5(decimal? d)
        {
            return Round(d, Precision005);
        }

        /// <summary>
        /// Round60s the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static decimal Round60(decimal d)
        {
            return Round(d, Precision060);
        }

        /// <summary>
        /// Round60s the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static decimal? Round60(decimal? d)
        {
            return Round(d, Precision060);
        }

        /// <summary>
        /// Round120s the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static decimal Round120(decimal d)
        {
            return Round(d, Precision120);
        }

        /// <summary>
        /// Round1s the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static decimal? Round1(decimal? d)
        {
            return Round(d, Precision001);
        }

        /// <summary>
        /// Round10s the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static decimal? Round10(decimal? d)
        {
            return Round(d, Precision010);
        }

        /// <summary>
        /// Round10s the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static decimal Round10(decimal d)
        {
            return Round(d, Precision010);
        }

        /// <summary>
        /// Round1s the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static decimal Round1(decimal d)
        {
            return Round(d, Precision001);
        }

        /// <summary>
        /// Round100s the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static decimal? Round100(decimal? d)
        {
            return Round(d, Precision100);
        }

        /// <summary>
        /// Interpolates the specified v1.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        public static decimal Interpol(decimal v1, decimal v2, decimal scale)
        {
            return v1 + (v2 - v1) * scale;
        }

        /// <summary>
        /// Interpolates the round.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="interpol">The interpol.</param>
        /// <returns></returns>
        public static decimal InterpolRound(decimal v1, decimal v2, decimal interpol)
        {
            return Round(v1 + (v2 - v1) * interpol, 1M);
        }

        /// <summary>
        /// Interpolates the round5.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="interpol">The interpol.</param>
        /// <returns></returns>
        public static decimal InterpolRound5(decimal v1, decimal v2, decimal interpol)
        {
            return Round5(v1 + (v2 - v1) * interpol);
        }

        /// <summary>
        /// Interpolates the round5.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="interpol">The interpol.</param>
        /// <returns></returns>
        public static decimal? InterpolRound5(decimal? v1, decimal v2, decimal interpol)
        {
            return Round5(v1 + (v2 - v1) * interpol);
        }

        /// <summary>
        /// Interpolates the round5.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="interpol">The interpol.</param>
        /// <returns></returns>
        public static decimal? InterpolRound5(decimal v1, decimal? v2, decimal interpol)
        {
            return Round5(v1 + (v2 - v1) * interpol);
        }

        /// <summary>
        /// Interpolates the round5.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="interpol">The interpol.</param>
        /// <returns></returns>
        public static decimal? InterpolRound5(decimal? v1, decimal? v2, decimal interpol)
        {
            return Round5(v1 + (v2 - v1) * interpol);
        }

        /// <summary>
        /// Interpolates the round60.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="interpol">The interpol.</param>
        /// <returns></returns>
        public static decimal InterpolRound60(decimal v1, decimal v2, decimal interpol)
        {
            return Round60(v1 + (v2 - v1) * interpol);
        }

        /// <summary>
        /// Interpolates the round120.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="interpol">The interpol.</param>
        /// <returns></returns>
        public static decimal InterpolRound120(decimal v1, decimal v2, decimal interpol)
        {
            return Round120(v1 + (v2 - v1) * interpol);
        }

        private const decimal Precision001 = 0.01M;
        private const decimal Precision005 = 0.05M;
        private const decimal Precision010 = 0.10M;
        private const decimal Precision060 = 0.60M;
        private const decimal Precision100 = 1.00M;
        private const decimal Precision120 = 0.60M;

        private const decimal PrecisionProgress = 0.000_000_0001M;
    }

}
