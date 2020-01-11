using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Basis.Wealth
{
    public class SGBasisWealthTaxCalculator : IBasisWealthTaxCalculator
    {
        public Task<Either<string, BasisTaxResult>> CalculateAsync(int calculationYear, Canton canton, BasisTaxPerson person)
        {
            throw new System.NotImplementedException();
        }
    }
}