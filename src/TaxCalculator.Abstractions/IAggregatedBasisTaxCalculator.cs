using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;


namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IAggregatedBasisTaxCalculator
    {
        Task<Either<string, AggregatedBasisTaxResult>> CalculateAsync(int calculationYear, TaxPerson person);
    }
}
