using Domain.Enums;

namespace Application.MultiPeriodCalculator.ExcelManager;

public record ExcelTransferRatioCashFlow(
    DateOnly ProcessDate,
    Guid DebitAccountId,
    Guid CreditAccountId,
    string Description,
    decimal TransferFactor,
    TaxType TaxType,
    FlowType FlowType)
    : ExcelCashFlowBase(ProcessDate, DebitAccountId, CreditAccountId, Description, TaxType, FlowType);
