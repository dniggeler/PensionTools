using Domain.Enums;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod
{
    public class SinglePeriodCalculationResult
    {
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public AccountType AccountType { get; set; }
    }
}
