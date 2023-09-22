using Application.Bvg;
using Application.Bvg.Models;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PensionCoach.Tools.BvgCalculator.Validators;


namespace PensionCoach.Tools.BvgCalculator;

public static class BvgCalculatorsCollectionExtensions
{
    public static void AddBvgCalculators(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IBvgRetirementCredits, BvgRetirementCreditsTable>();
        serviceCollection.AddSingleton<IBvgCalculator, BvgCalculator>();

        serviceCollection.AddSingleton<IValidator<BvgPerson>, BvgPersonValidator>();
    }
}
