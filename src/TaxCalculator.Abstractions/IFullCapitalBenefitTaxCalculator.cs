using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IFullCapitalBenefitTaxCalculator
    {
        Task<Either<string,FullCapitalBenefitTaxResult>> CalculateAsync(
            int calculationYear, int municipalityId, Canton canton, CapitalBenefitTaxPerson person);
    }
}
