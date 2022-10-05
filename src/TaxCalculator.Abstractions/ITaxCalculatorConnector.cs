using LanguageExt;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using System.Threading.Tasks;

namespace PensionCoach.Tools.TaxCalculator.Abstractions;

public interface ITaxCalculatorConnector
{
    Task<Either<string, FullTaxResult>> CalculateAsync(
        int calculationYear, int bfsMunicipalityId, TaxPerson person, bool withMaxAvailableCalculationYear = false);

    Task<Either<string, FullCapitalBenefitTaxResult>> CalculateAsync(
        int calculationYear, int bfsMunicipalityId, CapitalBenefitTaxPerson person, bool withMaxAvailableCalculationYear = false);

    Task<int[]> GetSupportedTaxYears();
}
