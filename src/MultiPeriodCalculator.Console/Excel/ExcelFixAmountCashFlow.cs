using Domain.Enums;

namespace MultiPeriodCalculator.Console.Excel;

public record ExcelFixAmountCashFlow(
    DateOnly ProcessDate, Guid DebitAccountId, Guid CreditAccountId, string Description, decimal Amount, TaxType TaxType, FlowType FlowType);
