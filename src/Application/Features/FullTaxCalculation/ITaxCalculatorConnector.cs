using Application.Tax.Proprietary.Abstractions.Models;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Features.FullTaxCalculation;

public interface ITaxCalculatorConnector
{
    Task<Either<string, FullTaxResult>> CalculateAsync(
        int calculationYear, int bfsMunicipalityId, TaxPerson person, bool withMaxAvailableCalculationYear = false);

    Task<Either<string, FullCapitalBenefitTaxResult>> CalculateAsync(
        int calculationYear, int bfsMunicipalityId, CapitalBenefitTaxPerson person, bool withMaxAvailableCalculationYear = false);

    Task<int[]> GetSupportedTaxYears();
}
