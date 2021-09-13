using Microsoft.Extensions.DependencyInjection;

namespace Calculators.CashFlow
{
    public static class CashFlowCalculatorsServiceCollectionExtensions
    {
        public static void AddCashFlowCalculators(this IServiceCollection collection)
        {
            collection.AddTransient<IMultiPeriodCashFlowCalculator, MultiPeriodCashFlowCalculator>();
        }
    }
}
