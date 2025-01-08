using Domain.Enums;

namespace CashFlowCalculationEngine.Server.Domain.CashFlows;

public record SingleCashFlow(
    DateOnly DateOfProcess,
    decimal Amount,
    Guid SourceAccountId,
    Guid TargetAccountId,
    bool IsTaxable,
    TaxType TaxType);
