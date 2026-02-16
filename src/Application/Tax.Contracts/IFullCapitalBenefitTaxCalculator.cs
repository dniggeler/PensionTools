using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Tax.Contracts;

public interface IFullCapitalBenefitTaxCalculator
{
    Task<Either<string, FullCapitalBenefitTaxResult>> CalculateAsync(
        int calculationYear,
        MunicipalityModel municipality,
        CapitalBenefitTaxPerson person,
        bool withMaxAvailableCalculationYear = false);

    Task<Either<string, FullCapitalBenefitTaxResult>> CalculateAsync(
        int calculationYear,
        long taxLocationId,
        CapitalBenefitTaxPerson person);
}
