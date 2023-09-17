using Domain.Enums;

namespace Domain.Models.MultiPeriod;

public class SinglePeriodCalculationResult
{
    public int Year { get; set; }
    public decimal Amount { get; set; }
    public AccountType AccountType { get; set; }
}
