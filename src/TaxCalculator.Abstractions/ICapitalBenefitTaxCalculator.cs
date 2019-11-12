using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;


namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface ICapitalBenefitTaxCalculator
    {
        Task<Either<string,StateTaxResult>> CalculateAsync(int calculationYear, CapitalBenefitTaxPerson person);
    }
}
