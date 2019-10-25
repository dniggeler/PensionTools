using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;


namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IWealthTaxCalculator
    {
        Task<Either<TaxResult,string>> CalculateAsync(TaxPerson person);
    }
}
