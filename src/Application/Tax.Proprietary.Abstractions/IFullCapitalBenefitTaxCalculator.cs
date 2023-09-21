using Application.Tax.Proprietary.Abstractions.Models;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Tax.Proprietary.Abstractions;

public interface IFullCapitalBenefitTaxCalculator
{
    Task<Either<string,FullCapitalBenefitTaxResult>> CalculateAsync(
        int calculationYear,
        MunicipalityModel municipality,
        CapitalBenefitTaxPerson person,
        bool withMaxAvailableCalculationYear = false);
}
