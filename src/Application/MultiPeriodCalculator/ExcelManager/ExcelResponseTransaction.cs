using Domain.Enums;

namespace Application.MultiPeriodCalculator.ExcelManager;

public record ExcelResponseTransaction(
    Guid AccountId,
    AccountType AccountType,
    string AccountName,
    string Description,
    decimal? Amount,
    DateOnly ProcessDate,
    FlowType FlowType);
