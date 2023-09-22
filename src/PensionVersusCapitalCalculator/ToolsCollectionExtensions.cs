using Application.Features.PensionVersusCapital;
using Microsoft.Extensions.DependencyInjection;

namespace PensionVersusCapitalCalculator;

public static class ToolsCollectionExtensions
{
    public static void AddToolsCalculators(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IPensionVersusCapitalCalculator, PensionVersusCapitalCalculator>();
    }
}
