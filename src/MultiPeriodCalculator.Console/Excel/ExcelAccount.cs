using Domain.Enums;

namespace MultiPeriodCalculator.Console.Excel;

public record ExcelAccount(Guid AccountId, string Name, AccountType AccountType);
