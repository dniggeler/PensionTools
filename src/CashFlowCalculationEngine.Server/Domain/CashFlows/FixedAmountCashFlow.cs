using Domain.Enums;

namespace CashFlowCalculationEngine.Server.Domain.CashFlows;

public record FixedAmountCashFlow(
    string? Description,
    DateOnly DateOfProcess,
    decimal Amount,
    int? Sequence,
    TaxType TaxType,
    FlowType TaxFlowType,
    Guid SourceAccountId,
    Guid TargetAccountId) : SingleCashFlow(Description, DateOfProcess, Sequence, TaxType, TaxFlowType, SourceAccountId, TargetAccountId);
