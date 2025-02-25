using Domain.Enums;

namespace Application.MultiPeriodCalculator.ExcelReader;

public record ExcelAccount(Guid AccountId, string Name, AccountType AccountType);
