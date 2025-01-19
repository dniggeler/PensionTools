using Domain.Enums;

namespace CashFlowCalculationEngine.Server.Domain.CashFlows;

public record BalanceGrowthCashFlow(
    string? Description,
    decimal NetReturn,
    DateOnly DateOfProcess,
    int? Sequence,
    TaxType TaxType,
    FlowType TaxFlowType,
    Guid SourceAccountId,
    Guid TargetAccountId) : SingleCashFlow(Description, DateOfProcess, Sequence, TaxType, TaxFlowType, SourceAccountId, TargetAccountId);
