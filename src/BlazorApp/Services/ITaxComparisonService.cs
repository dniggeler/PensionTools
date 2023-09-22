using System.Collections.Generic;
using Application.Features.TaxComparison.Models;

namespace BlazorApp.Services;

public interface ITaxComparisonService
{
    IAsyncEnumerable<TaxComparerResponse> CalculateAsync(CapitalBenefitTaxComparerRequest request);

    IAsyncEnumerable<TaxComparerResponse> CalculateAsync(IncomeAndWealthComparerRequest request);
}
