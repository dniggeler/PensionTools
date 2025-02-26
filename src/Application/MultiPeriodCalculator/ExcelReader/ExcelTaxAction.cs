using Domain.Enums;

namespace Application.MultiPeriodCalculator.ExcelReader;

public record ExcelTaxAction(
    Guid DebitAccountId,
    Guid CreditAccountId,
    string Description,
    DateOnly StartPeriodDateString,
    DateOnly EndPeriodDateString,
    TaxType TaxType);
