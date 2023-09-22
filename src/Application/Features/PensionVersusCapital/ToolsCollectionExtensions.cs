using Microsoft.Extensions.DependencyInjection;

namespace Application.Features.PensionVersusCapital;

public static class ToolsCollectionExtensions
{
    public static void AddToolsCalculators(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IPensionVersusCapitalCalculator, PensionVersusCapitalCalculator>();
    }
}
