using Application.Tax.Proprietary.Abstractions.Models;
using Domain.Enums;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Tax.Proprietary.Contracts;

public interface IIncomeTaxCalculator
{
    Task<Either<string, SingleTaxResult>> CalculateAsync(
        int calculationYear, int municipalityId, Canton canton, TaxPerson person);
}
