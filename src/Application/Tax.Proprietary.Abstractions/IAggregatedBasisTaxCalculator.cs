using Application.Tax.Proprietary.Abstractions.Models;
using Domain.Enums;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Tax.Proprietary.Abstractions
{
    public interface IAggregatedBasisTaxCalculator
    {
        Task<Either<string, AggregatedBasisTaxResult>> CalculateAsync(
            int calculationYear, Canton canton, TaxPerson person);
    }
}
