using Domain.Enums;

namespace MultiPeriodCalculator.Console.Excel;

public record ExcelAccountDefinition(Guid AccountId, string Name, AccountType AccountType);
