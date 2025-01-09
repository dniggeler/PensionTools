using Domain.Enums;

namespace CashFlowCalculationEngine.Server.Domain.CashFlows;

public record FixedAmountCashFlow(
    string? Description,
    DateOnly DateOfProcess,
    decimal Amount,
    Guid SourceAccountId,
    Guid TargetAccountId,
    TaxType TaxType) : SingleCashFlow(Description, DateOfProcess, SourceAccountId, TargetAccountId);
