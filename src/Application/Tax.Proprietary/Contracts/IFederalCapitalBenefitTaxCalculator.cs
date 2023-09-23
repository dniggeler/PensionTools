using Application.Tax.Proprietary.Abstractions.Models;
using Domain.Models.Tax.Person;
using LanguageExt;

namespace Application.Tax.Proprietary.Contracts;

public interface IFederalCapitalBenefitTaxCalculator
{
    Task<Either<string, BasisTaxResult>> CalculateAsync(
        int calculationYear, FederalTaxPerson person);
}
