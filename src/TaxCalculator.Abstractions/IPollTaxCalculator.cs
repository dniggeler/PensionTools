using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Tax.Data.Abstractions.Models;

namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IPollTaxCalculator
    {
        Task<Either<string, PollTaxResult>> CalculateAsync(
            int calculationYear,
            int municipalityId,
            Canton canton,
            PollTaxPerson person);

        Task<Either<string, PollTaxResult>> CalculateAsync(
            int calculationYear,
            Canton canton,
            PollTaxPerson person,
            TaxRateEntity taxRateEntity);
    }
}
