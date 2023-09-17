using System;
using Domain.Enums;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;

namespace Calculators.CashFlow.Models
{
    public record CashFlowModel(
        DateOnly DateOfProcess,
        decimal Amount,
        AccountType Source,
        AccountType Target,
        bool IsTaxable,
        TaxType TaxType);
}
