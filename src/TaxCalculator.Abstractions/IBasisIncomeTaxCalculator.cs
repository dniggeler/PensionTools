using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace PensionCoach.Tools.TaxCalculator.Abstractions;

public interface IBasisIncomeTaxCalculator
{
    Task<Either<string,BasisTaxResult>> CalculateAsync(
        int calculationYear, Canton canton, BasisTaxPerson person);
}
