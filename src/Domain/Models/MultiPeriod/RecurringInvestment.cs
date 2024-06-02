using Domain.Enums;

namespace Domain.Models.MultiPeriod
{
    public record RecurringInvestment
    {
        public decimal Amount { get; set; }
        public FrequencyType Frequency { get; set; }
    }
}
