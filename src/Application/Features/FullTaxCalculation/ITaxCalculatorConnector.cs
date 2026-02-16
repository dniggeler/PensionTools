using Domain.Models.Tax;
using LanguageExt;

namespace Application.Features.FullTaxCalculation
{
    public interface ITaxCalculatorConnector
    {
        Task<Either<string, FullTaxResult>> CalculateAsync(
            int calculationYear, int bfsMunicipalityId, TaxPerson person, bool withMaxAvailableCalculationYear = false);

        Task<Either<string, FullCapitalBenefitTaxResult>> CalculateAsync(
            int calculationYear, int bfsMunicipalityId, CapitalBenefitTaxPerson person, bool withMaxAvailableCalculationYear = false);

        Task<Either<string, FullTaxResult>> CalculateAsync(int calculationYear, long taxLocationId, TaxPerson person);

        Task<Either<string, FullCapitalBenefitTaxResult>> CalculateAsync(int calculationYear, long taxLocationId, CapitalBenefitTaxPerson person);

        Task<int[]> GetSupportedTaxYears();
    }
}
