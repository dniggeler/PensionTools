namespace CashFlowCalculationEngine.Server.Domain.CashFlows;

public record SingleCashFlow(
    string? Description,
    DateOnly DateOfProcess,
    int? Sequence,
    Guid SourceAccountId,
    Guid TargetAccountId);
