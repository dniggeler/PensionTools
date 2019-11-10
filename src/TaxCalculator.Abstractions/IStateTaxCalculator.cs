using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;


namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IStateTaxCalculator
    {
        Task<Either<string,StateTaxResult>> CalculateAsync(int calculationYear, TaxPerson person);
    }
}
