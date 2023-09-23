using Application.Tax.Proprietary.Abstractions.Models;
using Application.Tax.Proprietary.Abstractions.Models.Person;
using Application.Tax.Proprietary.Abstractions.Repositories;
using Application.Tax.Proprietary.Contracts;
using AutoMapper;
using Domain.Enums;
using Domain.Models.Tax;
using FluentValidation;
using LanguageExt;

namespace Application.Tax.Proprietary;

public class ProprietaryWealthTaxCalculator : IWealthTaxCalculator
{
    private readonly IValidator<TaxPerson> taxPersonValidator;
    private readonly IStateTaxRateRepository stateTaxRateRepository;
    private readonly Func<Canton, IBasisWealthTaxCalculator> basisWealthTaxCalculatorFunc;
    private readonly IMapper mapper;

    public ProprietaryWealthTaxCalculator(
        IValidator<TaxPerson> taxPersonValidator,
        IStateTaxRateRepository stateTaxRateRepository,
        Func<Canton, IBasisWealthTaxCalculator> basisWealthTaxCalculatorFunc,
        IMapper mapper)
    {
        this.taxPersonValidator = taxPersonValidator;
        this.stateTaxRateRepository = stateTaxRateRepository;
        this.basisWealthTaxCalculatorFunc = basisWealthTaxCalculatorFunc;
        this.mapper = mapper;
    }

    public async Task<Either<string, SingleTaxResult>> CalculateAsync(
        int calculationYear, int municipalityId, Canton canton, TaxPerson person)
    {
        var validationResult = taxPersonValidator.Validate(person);
        if (!validationResult.IsValid)
        {
            var errorMessageLine = string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
            return $"validation failed: {errorMessageLine}";
        }

        var basisTaxPerson = mapper.Map<BasisTaxPerson>(person);

        basisTaxPerson.TaxableAmount = person.TaxableWealth;

        var basisTaxResult =
            await basisWealthTaxCalculatorFunc(canton).CalculateAsync(
                calculationYear, canton, basisTaxPerson);

        return basisTaxResult
            .Match<Either<string, SingleTaxResult>>(
                Right: r => CalculateTax(calculationYear, municipalityId, r),
                Left: msg => msg);
    }

    private SingleTaxResult CalculateTax(
        int calculationYear, int municipalityId, BasisTaxResult basisTaxResult)
    {
        var taxRate = stateTaxRateRepository.TaxRates(calculationYear, municipalityId);

        return new SingleTaxResult
        {
            BasisTaxAmount = basisTaxResult,
            MunicipalityRate = taxRate.TaxRateMunicipality,
            CantonRate = taxRate.TaxRateCanton,
        };
    }
}
