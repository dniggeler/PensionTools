using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.CommonTypes.Tax;
using Tax.Tools.Comparison.Abstractions.Models;


namespace Tax.Tools.Comparison.Abstractions
{
    public interface ITaxComparer
    {
        Task<Either<string,IReadOnlyCollection<CapitalBenefitTaxComparerResult>>> CompareCapitalBenefitTaxAsync(
            CapitalBenefitTaxPerson person);

        IAsyncEnumerable<Either<string, CapitalBenefitTaxComparerResult>> CompareCapitalBenefitTaxAsync(
            CapitalBenefitTaxPerson person, int maxNumberOfMunicipality);
    }
}
