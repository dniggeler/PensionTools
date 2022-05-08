using System;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.Tax;

namespace Calculators.CashFlow.Models
{
    public record CashFlowModel(
        DateOnly DateOfOccurrence,
        decimal Amount,
        AccountType Source,
        AccountType Target,
        bool IsTaxable,
        TaxType TaxType,
        OccurrenceType OccurrenceType);
}
