using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Enums;
using Domain.Models.Tax;
using FluentValidation;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Tax.Data;

namespace PensionCoach.Tools.TaxCalculator.Proprietary;

public class ProprietaryWealthTaxCalculator : IWealthTaxCalculator
{
    private readonly IValidator<TaxPerson> taxPersonValidator;
    private readonly Func<TaxRateDbContext> rateDbContextFunc;
    private readonly Func<Canton, IBasisWealthTaxCalculator> basisWealthTaxCalculatorFunc;
    private readonly IMapper mapper;

    public ProprietaryWealthTaxCalculator(
        IValidator<TaxPerson> taxPersonValidator,
        Func<TaxRateDbContext> rateDbContextFunc,
        Func<Canton, IBasisWealthTaxCalculator> basisWealthTaxCalculatorFunc,
        IMapper mapper)
    {
        this.taxPersonValidator = taxPersonValidator;
        this.rateDbContextFunc = rateDbContextFunc;
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
        using var dbContext = rateDbContextFunc();
        var taxRate = dbContext.Rates.AsNoTracking()
            .Single(item => item.BfsId == municipalityId
                            && item.Year == calculationYear);

        return new SingleTaxResult
        {
            BasisTaxAmount = basisTaxResult,
            MunicipalityRate = taxRate.TaxRateMunicipality,
            CantonRate = taxRate.TaxRateCanton,
        };
    }
}
