using Domain.Enums;

namespace CashFlowCalculationEngine.Server.Domain.CashFlows;

public record TransferRatioCashFlow(
    string? Description,
    decimal TransferFactor,
    DateOnly DateOfProcess,
    int? Sequence,
    TaxType TaxType,
    FlowType TaxFlowType,
    Guid SourceAccountId,
    Guid TargetAccountId) : SingleCashFlow(Description, DateOfProcess, Sequence, TaxType, TaxFlowType, SourceAccountId, TargetAccountId);
