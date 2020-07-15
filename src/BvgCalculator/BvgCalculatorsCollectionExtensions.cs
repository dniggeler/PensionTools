using Microsoft.Extensions.DependencyInjection;


namespace PensionCoach.Tools.BvgCalculator
{
    public static class BvgCalculatorsCollectionExtensions
    {
        public static void AddBvgCalculators(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IBvgRetirementCredits, BvgRetirementCreditsTable>();
            serviceCollection.AddSingleton<IBvgCalculator, BvgCalculator>();
        }
    }
}
