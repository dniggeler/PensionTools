using Application.Mapping;
using Application.Municipality;
using Application.Tax.Contracts;
using Application.Tax.Estv;
using Application.Tax.Mock;
using Application.Validators;
using AutoMapper;
using Domain.Enums;
using Domain.Models.Tax;
using Domain.Models.Tax.Person;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Features.FullTaxCalculation;

public static class TaxCalculatorServiceCollectionExtensions
{
    public static void AddTaxCalculators(this IServiceCollection collection, ApplicationMode applicationMode)
    {
        collection.AddFullTaxCalculators(applicationMode);
            
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MappingProfile());
        });

        collection.AddSingleton(_ => mappingConfig.CreateMapper());

        collection.AddValidators();
    }

    private static void AddFullTaxCalculators(this IServiceCollection collection, ApplicationMode applicationMode)
    {
        switch (applicationMode)
        {
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
        }
    }

    private static void AddValidators(this IServiceCollection collection)
    {
        collection.AddSingleton<IValidator<CapitalBenefitTaxPerson>, CapitalBenefitsTaxPersonValidator>();
        collection.AddSingleton<IValidator<BasisTaxPerson>, BasisTaxPersonValidator>();
        collection.AddSingleton<IValidator<TaxPerson>, TaxPersonValidator>();
        collection.AddSingleton<IValidator<FederalTaxPerson>, FederalTaxPersonValidator>();
        collection.AddSingleton<IValidator<PollTaxPerson>, PollTaxPersonValidator>();
        collection.AddSingleton<IValidator<ChurchTaxPerson>, ChurchTaxPersonValidator>();
    }
}
