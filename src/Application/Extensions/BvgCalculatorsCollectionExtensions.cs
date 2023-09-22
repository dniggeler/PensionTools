﻿using Application.Bvg;
using Application.Validators;
using Domain.Models.Bvg;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class BvgCalculatorsCollectionExtensions
{
    public static void AddBvgCalculators(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IBvgRetirementCredits, BvgRetirementCreditsTable>();
        serviceCollection.AddSingleton<IBvgCalculator, BvgCalculator>();

        serviceCollection.AddSingleton<IValidator<BvgPerson>, BvgPersonValidator>();
    }
}