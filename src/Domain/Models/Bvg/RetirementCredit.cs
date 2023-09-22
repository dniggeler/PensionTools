namespace Domain.Models.Bvg
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

        public RetirementCredit(decimal amountRaw, int age)
        {
            AmountRaw = amountRaw;
            Age = age;
        }
    }
}
