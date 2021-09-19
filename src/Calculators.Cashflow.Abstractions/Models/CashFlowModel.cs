using PensionCoach.Tools.CommonTypes;

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
