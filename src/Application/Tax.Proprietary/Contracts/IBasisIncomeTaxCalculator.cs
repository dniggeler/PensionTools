using Application.Tax.Proprietary.Abstractions.Models;
using Domain.Enums;
using Domain.Models.Tax.Person;
using LanguageExt;

namespace Application.Tax.Proprietary.Contracts;

public interface IBasisIncomeTaxCalculator
{
    Task<Either<string, BasisTaxResult>> CalculateAsync(
        int calculationYear, Canton canton, BasisTaxPerson person);
}
