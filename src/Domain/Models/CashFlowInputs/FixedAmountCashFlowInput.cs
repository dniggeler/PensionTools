using Domain.Enums;

namespace Domain.Models.CashFlowInputs;

public record FixedAmountCashFlowInput(
    string? Description,
    DateOnly DateOfProcess,
    decimal Amount,
    int? Sequence,
    TaxType TaxType,
    FlowType TaxFlowType,
    Guid SourceAccountId,
    Guid TargetAccountId) : SingleCashFlowInput(Description, DateOfProcess, Sequence, TaxType, TaxFlowType, SourceAccountId, TargetAccountId);
