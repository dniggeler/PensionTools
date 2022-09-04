using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace PensionCoach.Tools.TaxCalculator.Abstractions;

public interface IFullWealthAndIncomeTaxCalculator
{
    Task<Either<string,FullTaxResult>> CalculateAsync(
        int calculationYear, int taxId, Canton canton, TaxPerson person, bool withMaxAvailableCalculationYear = false);
}
