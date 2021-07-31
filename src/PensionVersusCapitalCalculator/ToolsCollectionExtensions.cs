using Microsoft.Extensions.DependencyInjection;
using PensionVersusCapitalCalculator.Abstractions;

namespace PensionVersusCapitalCalculator
{
    public static class ToolsCollectionExtensions
    {
        public static void AddToolsCalculators(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IPensionVersusCapitalCalculator, PensionVersusCapitalCalculator>();
        }
    }
}
