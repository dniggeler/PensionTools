using System.Threading.Tasks;
using Domain.Models.Municipality;
using LanguageExt;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IFullCapitalBenefitTaxCalculator
    {
        Task<Either<string,FullCapitalBenefitTaxResult>> CalculateAsync(
            int calculationYear,
            MunicipalityModel municipality,
            CapitalBenefitTaxPerson person,
            bool withMaxAvailableCalculationYear = false);
    }
}
