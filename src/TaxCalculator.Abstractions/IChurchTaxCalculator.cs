using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;


namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IChurchTaxCalculator
    {
        Task<Either<string,ChurchTaxResult>> CalculateAsync(
            int calculationYear, TaxPerson person, BasisTaxResult basisIncomeTaxResult);
    }
}
