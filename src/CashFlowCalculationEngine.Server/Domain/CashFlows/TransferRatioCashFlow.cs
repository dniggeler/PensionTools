using Domain.Enums;

namespace CashFlowCalculationEngine.Server.Domain.CashFlows;

public record TransferRatioCashFlow(
    string? Description,
    decimal TransferFactor,
    TaxType TaxType,
    DateOnly DateOfProcess,
    Guid SourceAccountId,
    Guid TargetAccountId) : SingleCashFlow(Description, DateOfProcess, SourceAccountId, TargetAccountId);
