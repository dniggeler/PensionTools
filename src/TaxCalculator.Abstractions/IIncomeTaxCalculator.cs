using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IIncomeTaxCalculator
    {
        Task<Either<string, SingleTaxResult>> CalculateAsync(int calculationYear, TaxPerson person);
    }
}
