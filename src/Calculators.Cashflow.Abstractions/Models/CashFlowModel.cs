using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.Tax;

namespace Calculators.CashFlow.Models
{
    public record CashFlowModel(
        int Year,
        decimal Amount,
        AccountType Source,
        AccountType Target,
        bool IsTaxable,
        TaxType TaxType,
        OccurrenceType OccurrenceType);
}
