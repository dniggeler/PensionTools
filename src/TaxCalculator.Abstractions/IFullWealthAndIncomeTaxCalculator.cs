using System.Threading.Tasks;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace PensionCoach.Tools.TaxCalculator.Abstractions;

public interface IFullWealthAndIncomeTaxCalculator
{
    Task<Either<string,FullTaxResult>> CalculateAsync(
        int calculationYear, MunicipalityModel municipality, TaxPerson person, bool withMaxAvailableCalculationYear = false);
}
