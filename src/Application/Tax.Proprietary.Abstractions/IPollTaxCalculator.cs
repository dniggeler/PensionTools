using Application.Tax.Proprietary.Abstractions.Models;
using Application.Tax.Proprietary.Abstractions.Models.Person;
using Domain.Enums;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Tax.Proprietary.Abstractions
{
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
}
