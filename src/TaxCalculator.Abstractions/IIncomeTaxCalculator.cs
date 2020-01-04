using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;


namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IIncomeTaxCalculator
    {
        Task<Either<string, SingleTaxResult>> CalculateAsync(
            int calculationYear, int municipalityId, TaxPerson person);
    }
}
