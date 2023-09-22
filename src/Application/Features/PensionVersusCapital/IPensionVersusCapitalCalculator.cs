using Domain.Enums;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Features.PensionVersusCapital;

public interface IPensionVersusCapitalCalculator
{
    Task<Option<decimal>> CalculateAsync(
        int calculationYear,
        int municipalityId,
        Canton canton,
        decimal retirementPension,
        decimal retirementCapital,
        TaxPerson taxPerson);
}
