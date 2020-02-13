using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Tax.Tools.Comparison.Abstractions.Models;


namespace Tax.Tools.Comparison.Abstractions
{
    public interface ITaxComparer
    {
        Task<Either<string,IReadOnlyCollection<CapitalBenefitTaxComparerResult>>> CompareCapitalBenefitTaxAsync(
            int calculationYear, CapitalBenefitTaxPerson person);
    }
}
