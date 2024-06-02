using Domain.Models.Tax;
using Domain.Models.TaxComparison;
using LanguageExt;

namespace Application.Features.TaxComparison
{
    public interface ITaxComparer
    {
        IAsyncEnumerable<Either<string, CapitalBenefitTaxComparerResult>> CompareCapitalBenefitTaxAsync(
            CapitalBenefitTaxPerson person, int[] bfsNumbers);

        IAsyncEnumerable<Either<string, IncomeAndWealthTaxComparerResult>> CompareIncomeAndWealthTaxAsync(
            TaxPerson person, int[] bfsNumbers);
    }
}
