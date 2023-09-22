using Application.Bvg;
using Application.Validators;
using Domain.Models.Bvg;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace PensionCoach.Tools.BvgCalculator;

public static class BvgCalculatorsCollectionExtensions
{
    public static void AddBvgCalculators(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IBvgRetirementCredits, BvgRetirementCreditsTable>();
        serviceCollection.AddSingleton<IBvgCalculator, Application.Bvg.BvgCalculator>();

        serviceCollection.AddSingleton<IValidator<BvgPerson>, BvgPersonValidator>();
    }
}
