using Domain.Enums;

namespace Domain.Models.CashFlowInputs;

public record TransferRatioCashFlowInput(
    string? Description,
    decimal TransferFactor,
    DateOnly DateOfProcess,
    int? Sequence,
    TaxType TaxType,
    FlowType TaxFlowType,
    Guid SourceAccountId,
    Guid TargetAccountId) : SingleCashFlowInput(Description, DateOfProcess, Sequence, TaxType, TaxFlowType, SourceAccountId, TargetAccountId);
