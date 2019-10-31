using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;


namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IFederalTaxCalculator
    {
        Task<Either<string,BasisTaxResult>> CalculateAsync(int calculationYear, BasisTaxPerson person);
    }
}
