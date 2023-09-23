using Domain.Enums;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Tax.Proprietary.Contracts;

public interface ICapitalBenefitTaxCalculator
{
    Task<Either<string, CapitalBenefitTaxResult>> CalculateAsync(
        int calculationYear,
        int taxId,
        Canton canton,
        CapitalBenefitTaxPerson person);
}
