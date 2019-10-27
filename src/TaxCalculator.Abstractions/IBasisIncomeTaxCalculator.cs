using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;


namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IBasisIncomeTaxCalculator
    {
        Task<Either<BasisTaxResult,string>> CalculateAsync(int calculationYear, BasisTaxPerson person);
    }
}
