using Microsoft.Extensions.DependencyInjection;

namespace Application.Features.TaxComparison
{
    public static class TaxComparerServiceCollectionExtensions
    {
        public static void AddTaxComparers(this IServiceCollection collection)
        {
            collection.AddTransient<ITaxComparer, TaxComparer>();
        }
    }
}
