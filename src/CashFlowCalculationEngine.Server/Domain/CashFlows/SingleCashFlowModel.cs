using Domain.Enums;

namespace CashFlowCalculationEngine.Server.Domain.CashFlows;

public record CashFlowModel(
    DateOnly DateOfProcess,
    decimal Amount,
    Guid SourceAccountId,
    Guid TargetAccountId,
    bool IsTaxable,
    TaxType TaxType);
