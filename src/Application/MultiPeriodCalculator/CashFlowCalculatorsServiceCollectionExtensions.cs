using Application.Features.TaxScenarios;
using Microsoft.Extensions.DependencyInjection;

namespace Application.MultiPeriodCalculator;

public static class CashFlowCalculatorsServiceCollectionExtensions
{
    public static void AddCashFlowCalculators(this IServiceCollection collection)
    {
        collection.AddTransient<IMultiPeriodCashFlowCalculator, MultiPeriodCashFlowCalculator>();
        collection.AddTransient<ITaxScenarioCalculator, TaxScenarioCalculator>();
    }
}
