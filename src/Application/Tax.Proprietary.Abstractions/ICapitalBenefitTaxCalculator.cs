using Application.Tax.Proprietary.Abstractions.Models;
using Domain.Enums;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Tax.Proprietary.Abstractions;

public interface ICapitalBenefitTaxCalculator
{
    Task<Either<string, CapitalBenefitTaxResult>> CalculateAsync(
        int calculationYear,
        int taxId,
        Canton canton,
        CapitalBenefitTaxPerson person);
}
