namespace PensionCoach.Tools.CommonUtils
{
    /// <summary>
    /// Pows the specified exponent.
    /// </summary>
    public static class DecimalExtensions
    {
        /// <summary>
        /// Pows the specified exponent.
        /// </summary>
        /// <param name="base">The base.</param>
        /// <param name="exponent">The exponent.</param>
        /// <returns></returns>
        public static decimal Pow(this decimal @base, int exponent)
        {
            if (exponent == 0)
            {
                return 1;
            }

            decimal result = @base;
            for (int iteration = 1; iteration < exponent; ++iteration)
            {
                result *= @base;
            }

            return result;
        }
    }
}
