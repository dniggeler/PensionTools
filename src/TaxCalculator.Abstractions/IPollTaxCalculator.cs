using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;


namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IPollTaxCalculator
    {
        Task<Either<string,decimal>> CalculateAsync(int calculationYear, PollTaxPerson person);
    }
}
