using Microsoft.Extensions.DependencyInjection;
using Tax.Tools.Comparison.Abstractions;


namespace Tax.Tools.Comparison
{
    public static class TaxComparerServiceCollectionExtensions
    {
        public static void AddTaxComparers(this IServiceCollection collection)
        {
            collection.AddTransient<ITaxComparer, TaxComparer>();
        }
    }
}