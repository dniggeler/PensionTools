using System;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.CommonUtils;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using PensionCoach.Tools.TaxCalculator.Basis.CapitalBenefit;
using PensionCoach.Tools.TaxCalculator.Basis.Income;
using PensionCoach.Tools.TaxCalculator.Basis.Wealth;
using PensionCoach.Tools.TaxCalculator.Estv;
using PensionCoach.Tools.TaxCalculator.Mapping;
using PensionCoach.Tools.TaxCalculator.Proprietary;
using PensionCoach.Tools.TaxCalculator.Validators;

namespace PensionCoach.Tools.TaxCalculator
{
    public static class TaxCalculatorServiceCollectionExtensions
    {
        public static void AddTaxCalculators(this IServiceCollection collection, IConfiguration configuration)
        {
            TypeOfTaxCalculator typeOfTaxCalculator = configuration.GetTypeOfTaxCalculator();

            collection.AddTransient<IIncomeTaxCalculator, IncomeTaxCalculator>();
            collection.AddTransient<IWealthTaxCalculator, WealthTaxCalculator>();
            collection.AddTransient<IFederalCapitalBenefitTaxCalculator, FederalCapitalBenefitTaxCalculator>();
            collection.AddTransient<IFederalTaxCalculator, FederalTaxCalculator>();
            collection.AddTransient<IAggregatedBasisTaxCalculator, AggregatedBasisTaxCalculator>();
            collection.AddTransient<IStateTaxCalculator, StateTaxCalculator>();
            if (typeOfTaxCalculator == TypeOfTaxCalculator.PensionTools)
            {
                collection.AddTransient<IFullWealthAndIncomeTaxCalculator, ProprietaryFullTaxCalculator>();
                collection.AddTransient<IFullCapitalBenefitTaxCalculator, ProprietaryFullCapitalBenefitTaxCalculator>();
                collection.AddTransient<IMunicipalityConnector, MunicipalityConnector>();
            }
            else
            {
                collection.AddEstvCalculators();
            }
            
            collection.AddTransient<ITaxCalculatorConnector, TaxCalculatorConnector>();
            collection.AddTransient<IAdminConnector, AdminConnector>();
            
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

        private static void AddEstvCalculators(this IServiceCollection collection)
        {
            collection.AddTransient<IFullWealthAndIncomeTaxCalculator, EstvFullTaxCalculator>();
            collection.AddTransient<IFullCapitalBenefitTaxCalculator, EstvFullCapitalBenefitTaxCalculator>();
            collection.AddTransient<IMunicipalityConnector, EstvMunicipalityConnector>();
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
            collection.AddTransient<IChurchTaxCalculator, ChurchTaxCalculator>();
            collection.AddTransient<IPollTaxCalculator, PollTaxCalculator>();
            collection.AddTransient<IDefaultBasisIncomeTaxCalculator, DefaultBasisIncomeTaxCalculator>();
        }
    }
}
