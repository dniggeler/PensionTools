using System.Collections.Generic;
using Domain.Models.Tax;
using LanguageExt;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxComparison;

namespace Tax.Tools.Comparison.Abstractions
{
    public interface ITaxComparer
    {
        IAsyncEnumerable<Either<string, CapitalBenefitTaxComparerResult>> CompareCapitalBenefitTaxAsync(
            CapitalBenefitTaxPerson person, int[] bfsNumbers);

        IAsyncEnumerable<Either<string, IncomeAndWealthTaxComparerResult>> CompareIncomeAndWealthTaxAsync(
            TaxPerson person, int[] bfsNumbers);
    }
}
