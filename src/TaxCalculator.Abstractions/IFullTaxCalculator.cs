using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;


namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IFullTaxCalculator
    {
        Task<Either<string,FullTaxResult>> CalculateAsync(int calculationYear, TaxPerson person);
    }
}
