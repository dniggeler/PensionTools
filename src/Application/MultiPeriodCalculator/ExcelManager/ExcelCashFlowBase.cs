using Domain.Enums;

namespace Application.MultiPeriodCalculator.ExcelManager;

public record ExcelCashFlowBase(
    DateOnly ProcessDate, Guid DebitAccountId, Guid CreditAccountId, string Description, TaxType TaxType, FlowType FlowType);
