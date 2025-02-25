using Domain.Enums;

namespace Application.MultiPeriodCalculator.ExcelReader;

public record ExcelFixAmountCashFlow(
    DateOnly ProcessDate, Guid DebitAccountId, Guid CreditAccountId, string Description, decimal Amount, TaxType TaxType, FlowType FlowType);
