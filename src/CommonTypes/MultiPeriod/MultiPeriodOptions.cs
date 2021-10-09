namespace PensionCoach.Tools.CommonTypes.MultiPeriod
{
    public class MultiPeriodOptions
    {
        private const decimal DefaultSalaryGrowthRate = 0.01M;
        private const decimal DefaultWealthGrowthRate = decimal.Zero;
        private const decimal DefaultSavingsQuota = 0.30M;

        public decimal SalaryNetGrowthRate { get; set; } = DefaultSalaryGrowthRate;

        public decimal WealthNetGrowthRate { get; set; } = DefaultWealthGrowthRate;

        /// <summary>
        /// Gets or sets the savings quota. Defines how much of the yearly made income
        /// is left and enters savings.
        /// </summary>
        /// <value>
        /// The savings quota.
        /// </value>
        public decimal SavingsQuota { get; set; } = DefaultSavingsQuota;
    }
}
