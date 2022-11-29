using System.Collections.Generic;
using PensionCoach.Tools.TaxComparison;

namespace BlazorApp.Services;

public interface ITaxComparisonService
{
    IAsyncEnumerable<TaxComparerResponse> CalculateAsync(CapitalBenefitTaxComparerRequest request);

    IAsyncEnumerable<TaxComparerResponse> CalculateAsync(IncomeAndWealthComparerRequest request);
}
