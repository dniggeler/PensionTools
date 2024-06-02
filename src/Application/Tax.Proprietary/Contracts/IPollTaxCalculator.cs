using Application.Tax.Proprietary.Models;
using Domain.Enums;
using Domain.Models.Tax;
using Domain.Models.Tax.Person;
using LanguageExt;

namespace Application.Tax.Proprietary.Contracts;

public interface IPollTaxCalculator
{
    Task<Either<string, PollTaxResult>> CalculateAsync(
        int calculationYear,
        int municipalityId,
        Canton canton,
        PollTaxPerson person);

    Task<Either<string, PollTaxResult>> CalculateAsync(
        int calculationYear,
        Canton canton,
        PollTaxPerson person,
        TaxRateEntity taxRateEntity);
}
