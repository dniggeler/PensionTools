using Domain.Enums;

namespace CashFlowCalculationEngine.Server.Domain.CashFlows;

public record FixedAmountCashFlow(
    string? Description,
    DateOnly DateOfProcess,
    decimal Amount,
    int? Sequence,
    Guid SourceAccountId,
    Guid TargetAccountId,
    TaxType TaxType) : SingleCashFlow(Description, DateOfProcess, Sequence, SourceAccountId, TargetAccountId);
