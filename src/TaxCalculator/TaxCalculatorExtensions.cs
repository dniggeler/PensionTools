using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using TaxCalculator.Validators;

namespace TaxCalculator
{
    public static class TaxCalculatorExtensions
    {
        public static void AddTaxCalculators(this ServiceCollection collection)
        {
            collection.AddTransient<IIncomeTaxCalculator, IncomeTaxCalculator>();
            collection.AddTransient<IWealthTaxCalculator, WealthTaxCalculator>();
            collection.AddTransient<IFederalTaxCalculator, FederalTaxCalculator>();
            collection.AddTransient<IAggregatedBasisTaxCalculator, AggregatedBasisTaxCalculator>();
            collection.AddTransient<IStateTaxCalculator , StateTaxCalculator>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            collection.AddSingleton(_ => mappingConfig.CreateMapper());

            collection.AddValidators();
            collection.AddBasisCalculators();
        }

        private static void AddValidators(this ServiceCollection collection)
        {
            collection.AddSingleton<IValidator<BasisTaxPerson>, BasisTaxPersonValidator>();
            collection.AddSingleton<IValidator<TaxPerson>, TaxPersonValidator>();
        }

        private static void AddBasisCalculators(this ServiceCollection collection)
        {
            collection.AddTransient<IBasisIncomeTaxCalculator, BasisIncomeTaxCalculator>();
            collection.AddTransient<IBasisWealthTaxCalculator, BasisWealthTaxCalculator>();
        }
    }
}