namespace CashFlowCalculationEngine.Server.Domain.CashFlows;

public record SingleCashFlow(
    string? Description,
    DateOnly DateOfProcess,
    Guid SourceAccountId,
    Guid TargetAccountId);
