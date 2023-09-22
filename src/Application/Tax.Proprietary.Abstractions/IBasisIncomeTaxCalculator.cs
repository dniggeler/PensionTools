using Application.Tax.Proprietary.Abstractions.Models;
using Application.Tax.Proprietary.Abstractions.Models.Person;
using Domain.Enums;
using LanguageExt;

namespace Application.Tax.Proprietary.Abstractions
{
    public interface IBasisIncomeTaxCalculator
    {
        Task<Either<string,BasisTaxResult>> CalculateAsync(
            int calculationYear, Canton canton, BasisTaxPerson person);
    }
}
