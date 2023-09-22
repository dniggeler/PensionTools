using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Tax.Proprietary.Abstractions;

public interface IFullWealthAndIncomeTaxCalculator
{
    Task<Either<string,FullTaxResult>> CalculateAsync(
        int calculationYear, MunicipalityModel municipality, TaxPerson person, bool withMaxAvailableCalculationYear = false);
}
