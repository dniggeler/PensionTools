namespace TaxCalculator
{
    using System.Threading.Tasks;
    using LanguageExt;
    using PensionCoach.Tools.TaxCalculator.Abstractions;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

    public class ClasCapitalBenefitTaxCalculator : ICapitalBenefitTaxCalculator
    {
        public Task<Either<string, StateTaxResult>> CalculateAsync(int calculationYear, CapitalBenefitTaxPerson person)
        {
            throw new System.NotImplementedException();
        }
    }
}