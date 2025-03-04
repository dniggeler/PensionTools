using Domain.Enums;

namespace Application.MultiPeriodCalculator.ExcelManager;

public record ExcelAccount(Guid AccountId, string Name, AccountType AccountType);
