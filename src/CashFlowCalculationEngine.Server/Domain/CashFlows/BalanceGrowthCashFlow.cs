using Domain.Enums;

namespace CashFlowCalculationEngine.Server.Domain.CashFlows;

public record BalanceGrowthCashFlow(
    string? Description,
    decimal NetReturn,
    DateOnly DateOfProcess,
    int? Sequence,
    TaxType TaxType,
    Guid SourceAccountId,
    Guid TargetAccountId) : SingleCashFlow(Description, DateOfProcess, Sequence, TaxType, SourceAccountId, TargetAccountId);
