using Domain.Enums;
using PensionCoach.Tools.CommonTypes.Tax;

namespace Domain.Models.Cashflows;

public record CashFlowModel(
    DateOnly DateOfProcess,
    decimal Amount,
    AccountType Source,
    AccountType Target,
    bool IsTaxable,
    TaxType TaxType);
