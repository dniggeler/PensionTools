using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models.Tax;
using LanguageExt;

namespace PensionVersusCapitalCalculator.Abstractions;

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
