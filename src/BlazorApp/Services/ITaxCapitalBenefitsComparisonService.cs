using System.Collections.Generic;
using PensionCoach.Tools.TaxComparison;

namespace BlazorApp.Services
{
    public interface ITaxCapitalBenefitsComparisonService
    {
        IAsyncEnumerable<CapitalBenefitTaxComparerResponse> CalculateAsync(CapitalBenefitTaxComparerRequest request);
    }
}
