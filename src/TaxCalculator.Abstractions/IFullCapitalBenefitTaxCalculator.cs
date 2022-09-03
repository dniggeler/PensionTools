using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IFullCapitalBenefitTaxCalculator
    {
        Task<Either<string,FullCapitalBenefitTaxResult>> CalculateAsync(
            int calculationYear,
            int taxId,
            Canton canton,
            CapitalBenefitTaxPerson person,
            bool withMaxAvailableCalculationYear = false);
    }
}
