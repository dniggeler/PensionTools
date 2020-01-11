using System;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using TaxCalculator.Basis.Income;
using TaxCalculator.Basis.Wealth;
using TaxCalculator.Mapping;
using TaxCalculator.Validators;

namespace TaxCalculator
{
    public static class TaxCalculatorExtensions
    {
        public static void AddTaxCalculators(this IServiceCollection collection)
        {
            collection.AddTransient<IIncomeTaxCalculator, IncomeTaxCalculator>();
            collection.AddTransient<IWealthTaxCalculator, WealthTaxCalculator>();
            collection.AddTransient<IFederalCapitalBenefitTaxCalculator,
                FederalCapitalBenefitTaxCalculator>();
            collection.AddTransient<IFederalTaxCalculator, FederalTaxCalculator>();
            collection.AddTransient<IAggregatedBasisTaxCalculator, AggregatedBasisTaxCalculator>();
            collection.AddTransient<IStateTaxCalculator, StateTaxCalculator>();
            collection.AddTransient<IFullTaxCalculator, FullTaxCalculator>();
            collection.AddTransient<IFullCapitalBenefitTaxCalculator, FullCapitalBenefitTaxCalculator>();

            collection.AddTransient<IMunicipalityConnector, MunicipalityConnector>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            collection.AddSingleton(_ => mappingConfig.CreateMapper());

            collection.AddValidators();
            collection.AddBasisCalculators();
            collection.AddCantonBasisTaxCalculatorFactory();
        }

        private static void AddCantonBasisTaxCalculatorFactory(this IServiceCollection collection)
        {
            collection.AddTransient<SGBasisIncomeTaxCalculator>();
            collection.AddTransient<MissingBasisIncomeTaxCalculator>();

            collection.AddSingleton<Func<Canton, IBasisIncomeTaxCalculator>>(ctx =>
                canton => canton switch {
                    Canton.SG => ctx.GetRequiredService<SGBasisIncomeTaxCalculator>(),
                    Canton.ZH => ctx.GetRequiredService<IDefaultBasisIncomeTaxCalculator>(),
                    _ => ctx.GetRequiredService<MissingBasisIncomeTaxCalculator>()
                });
        }

        private static void AddValidators(this IServiceCollection collection)
        {
            collection.AddSingleton<IValidator<CapitalBenefitTaxPerson>, CapitalBenefitsTaxPersonValidator>();
            collection.AddSingleton<IValidator<BasisTaxPerson>, BasisTaxPersonValidator>();
            collection.AddSingleton<IValidator<TaxPerson>, TaxPersonValidator>();
            collection.AddSingleton<IValidator<FederalTaxPerson>, FederalTaxPersonValidator>();
            collection.AddSingleton<IValidator<PollTaxPerson>, PollTaxPersonValidator>();
            collection.AddSingleton<IValidator<ChurchTaxPerson>, ChurchTaxPersonValidator>();
            collection.AddSingleton<IValidator<AggregatedBasisTaxResult>, AggregatedTaxResultValidator>();
        }

        private static void AddBasisCalculators(this IServiceCollection collection)
        {
            collection.AddTransient<IChurchTaxCalculator, ChurchTaxCalculator>();
            collection.AddTransient<IPollTaxCalculator, PollTaxCalculator>();
            collection.AddTransient<IDefaultBasisIncomeTaxCalculator, DefaultBasisIncomeTaxCalculator>();
            collection.AddTransient<IBasisWealthTaxCalculator, DefaultBasisWealthTaxCalculator>();
            collection.AddTransient<ICapitalBenefitTaxCalculator, CapitalBenefitTaxCalculator>();
        }
    }
}