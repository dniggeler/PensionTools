using Domain.Enums;

namespace PensionCoach.Tools.CommonTypes.MultiPeriod;

public record RecurringInvestment
{
    public decimal Amount { get; set; }
    public FrequencyType Frequency { get; set; }
}
