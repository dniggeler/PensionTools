using Application.Features.MarginalTaxCurve;
using Application.Mapping;
using Application.Municipality;
using Application.Tax.Estv;
using Application.Tax.Mock;
using Application.Tax.Proprietary;
using Application.Tax.Proprietary.Abstractions.Models;
using Application.Tax.Proprietary.Abstractions.Models.Person;
using Application.Tax.Proprietary.Basis.CapitalBenefit;
using Application.Tax.Proprietary.Basis.Income;
using Application.Tax.Proprietary.Basis.Wealth;
using Application.Tax.Proprietary.Contracts;
using Application.Validators;
using AutoMapper;
using Domain.Enums;
using Domain.Models.Tax;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Features.FullTaxCalculation
{
    public static class TaxCalculatorServiceCollectionExtensions
    {
        public static void AddTaxCalculators(this IServiceCollection collection, ApplicationMode applicationMode)
        {
            collection.AddTransient<IIncomeTaxCalculator, ProprietaryIncomeTaxCalculator>();
            collection.AddTransient<IWealthTaxCalculator, ProprietaryWealthTaxCalculator>();
            collection.AddTransient<IFederalCapitalBenefitTaxCalculator, ProprietaryFederalCapitalBenefitTaxCalculator>();
            collection.AddTransient<IFederalTaxCalculator, ProprietaryFederalTaxCalculator>();
            collection.AddTransient<IAggregatedBasisTaxCalculator, ProprietaryAggregatedBasisTaxCalculator>();
            collection.AddTransient<IStateTaxCalculator, ProprietaryStateTaxCalculator>();
            collection.AddTransient<ITaxCalculatorConnector, TaxCalculatorConnector>();
            collection.AddTransient<IMarginalTaxCurveCalculatorConnector, MarginalTaxCurveCalculatorConnector>();

            collection.AddFullTaxCalculators(applicationMode);
            
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            collection.AddSingleton(_ => mappingConfig.CreateMapper());

            collection.AddValidators();
            collection.AddBasisCalculators();
            collection.AddCantonIncomeTaxCalculatorFactory();
            collection.AddCantonWealthTaxCalculatorFactory();
            collection.AddCantonCapitalBenefitTaxCalculatorFactory();
        }

        private static void AddFullTaxCalculators(this IServiceCollection collection, ApplicationMode applicationMode)
        {
            switch (applicationMode)
            {
                case ApplicationMode.Proprietary:
                    collection.AddTransient<IFullWealthAndIncomeTaxCalculator, ProprietaryFullTaxCalculator>();
                    collection.AddTransient<IFullCapitalBenefitTaxCalculator, ProprietaryFullCapitalBenefitTaxCalculator>();
                    collection.AddTransient<IMunicipalityConnector, ProprietaryMunicipalityConnector>();
                    collection.AddTransient<ITaxSupportedYearProvider, ProprietarySupportedTaxYears>();
                    break;
                case ApplicationMode.Estv:
                    collection.AddTransient<IFullWealthAndIncomeTaxCalculator, EstvFullTaxCalculator>();
                    collection.AddTransient<IFullCapitalBenefitTaxCalculator, EstvFullCapitalBenefitTaxCalculator>();
                    collection.AddTransient<IMunicipalityConnector, EstvMunicipalityConnector>();
                    collection.AddTransient<ITaxSupportedYearProvider, EstvTaxSupportedYearProvider>();
                    break;
                case ApplicationMode.Mock:
                    collection.AddTransient<IFullCapitalBenefitTaxCalculator, MockedFullTaxCalculator>();
                    collection.AddTransient<IFullWealthAndIncomeTaxCalculator, MockedFullTaxCalculator>();
                    collection.AddTransient<IMunicipalityConnector, MockedFullTaxCalculator>();
                    collection.AddTransient<ITaxSupportedYearProvider, MockedFullTaxCalculator>();
                    break;
                default:
                    collection.AddTransient<IFullWealthAndIncomeTaxCalculator, ProprietaryFullTaxCalculator>();
                    collection.AddTransient<IFullCapitalBenefitTaxCalculator, ProprietaryFullCapitalBenefitTaxCalculator>();
                    collection.AddTransient<IMunicipalityConnector, ProprietaryMunicipalityConnector>();
                    collection.AddTransient<ITaxSupportedYearProvider, ProprietarySupportedTaxYears>();
                    break;
            }
        }

        private static void AddCantonIncomeTaxCalculatorFactory(
            this IServiceCollection collection)
        {
            collection.AddTransient<SGBasisIncomeTaxCalculator>();
            collection.AddTransient<SOBasisIncomeTaxCalculator>();
            collection.AddTransient<MissingBasisIncomeTaxCalculator>();

            collection.AddSingleton<Func<Canton, IBasisIncomeTaxCalculator>>(ctx =>
                canton => canton switch {
                    Canton.SO => ctx.GetRequiredService<SOBasisIncomeTaxCalculator>(),
                    Canton.SG => ctx.GetRequiredService<SGBasisIncomeTaxCalculator>(),
                    Canton.ZH => ctx.GetRequiredService<IDefaultBasisIncomeTaxCalculator>(),
                    _ => ctx.GetRequiredService<MissingBasisIncomeTaxCalculator>()
                });
        }

        private static void AddCantonCapitalBenefitTaxCalculatorFactory(
            this IServiceCollection collection)
        {
            collection.AddTransient<SOCapitalBenefitTaxCalculator>();
            collection.AddTransient<SGCapitalBenefitTaxCalculator>();
            collection.AddTransient<ZHCapitalBenefitTaxCalculator>();
            collection.AddTransient<MissingCapitalBenefitTaxCalculator>();

            collection.AddSingleton<Func<Canton, ICapitalBenefitTaxCalculator>>(ctx =>
                canton => canton switch
                {
                    Canton.SO => ctx.GetRequiredService<SOCapitalBenefitTaxCalculator>(),
                    Canton.SG => ctx.GetRequiredService<SGCapitalBenefitTaxCalculator>(),
                    Canton.ZH => ctx.GetRequiredService<ZHCapitalBenefitTaxCalculator>(),
                    _ => ctx.GetRequiredService<MissingCapitalBenefitTaxCalculator>()
                });
        }

        private static void AddCantonWealthTaxCalculatorFactory(
            this IServiceCollection collection)
        {
            collection.AddTransient<SOBasisWealthTaxCalculator>();
            collection.AddTransient<SGBasisWealthTaxCalculator>();
            collection.AddTransient<ZHBasisWealthTaxCalculator>();
            collection.AddTransient<MissingBasisWealthTaxCalculator>();

            collection.AddSingleton<Func<Canton, IBasisWealthTaxCalculator>>(ctx =>
                canton => canton switch {
                    Canton.SO => ctx.GetRequiredService<SOBasisWealthTaxCalculator>(),
                    Canton.SG => ctx.GetRequiredService<SGBasisWealthTaxCalculator>(),
                    Canton.ZH => ctx.GetRequiredService<ZHBasisWealthTaxCalculator>(),
                    _ => ctx.GetRequiredService<MissingBasisWealthTaxCalculator>()
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
            collection.AddTransient<IChurchTaxCalculator, ProprietaryChurchTaxCalculator>();
            collection.AddTransient<IPollTaxCalculator, ProprietaryPollTaxCalculator>();
            collection.AddTransient<IDefaultBasisIncomeTaxCalculator, DefaultBasisIncomeTaxCalculator>();
        }
    }
}
