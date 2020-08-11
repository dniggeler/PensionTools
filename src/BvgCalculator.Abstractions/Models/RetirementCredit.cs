using PensionCoach.Tools.CommonUtils;

namespace PensionCoach.Tools.BvgCalculator.Models
{
    /// <summary>
    /// Holds retirement credit
    /// </summary>
    public class RetirementCredit
    {
        /// <summary>
        /// Gets the age.
        /// </summary>
        /// <value>
        /// The age.
        /// </value>
        public int Age { get; set; }

        /// <summary>
        /// BVG portion
        /// </summary>
        public decimal AmountRaw { get; set; }

        /// <summary>
        /// Gets the amount rounded by 10 (up to 0.1 CHF).
        /// </summary>
        /// <value>
        /// The amount rounded10.
        /// </value>
        public decimal AmountRounded10 => MathUtils.Round10(AmountRaw);

        /// <summary>
        /// Gets the amount rounded by 60 (up to 0.60 CHF).
        /// </summary>
        /// <value>
        /// The amount rounded60.
        /// </value>
        public decimal AmountRounded60 => MathUtils.Round60(AmountRaw);

        /// <summary>
        /// Gets the amount rounded up to 100.
        /// </summary>
        /// <value>
        /// The amount rounded100.
        /// </value>
        public decimal AmountRounded100 => MathUtils.Round(AmountRaw);

        public RetirementCredit()
        {
            
        }

        public RetirementCredit(decimal amountRaw, int age)
        {
            AmountRaw = amountRaw;
            Age = age;
        }
    }
}
