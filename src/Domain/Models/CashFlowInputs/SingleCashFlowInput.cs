using Domain.Enums;

namespace Domain.Models.CashFlowInputs;

public record SingleCashFlowInput(
    string? Description,
    DateOnly DateOfProcess,
    int? Sequence,
    TaxType TaxType,
    FlowType TaxFlowType,
    Guid SourceAccountId,
    Guid TargetAccountId);
