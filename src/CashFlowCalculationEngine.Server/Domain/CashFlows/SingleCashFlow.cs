using Domain.Enums;

namespace CashFlowCalculationEngine.Server.Domain.CashFlows;

public record SingleCashFlow(
    string? Description,
    DateOnly DateOfProcess,
    int? Sequence,
    TaxType TaxType,
    FlowType TaxFlowType,
    Guid SourceAccountId,
    Guid TargetAccountId);
