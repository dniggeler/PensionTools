using Domain.Enums;

namespace CashFlowCalculationEngine.Server.Domain.CashFlows;

public record TransferRatioCashFlow(
    string? Description,
    decimal TransferFactor,
    TaxType TaxType,
    DateOnly DateOfProcess,
    int? Sequence,
    Guid SourceAccountId,
    Guid TargetAccountId) : SingleCashFlow(Description, DateOfProcess, Sequence, SourceAccountId, TargetAccountId);
