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

public class ProprietaryIncomeTaxCalculator : IIncomeTaxCalculator
{
    private readonly IValidator<TaxPerson> taxPersonValidator;
    private readonly Func<TaxRateDbContext> rateDbContextFunc;
    private readonly Func<Canton, IBasisIncomeTaxCalculator> basisIncomeTaxCalculatorFunc;
    private readonly IMapper mapper;

    public ProprietaryIncomeTaxCalculator(
        IValidator<TaxPerson> taxPersonValidator,
        Func<TaxRateDbContext> rateDbContextFunc,
        Func<Canton, IBasisIncomeTaxCalculator> basisIncomeTaxCalculatorFunc,
        IMapper mapper)
    {
        this.taxPersonValidator = taxPersonValidator;
        this.rateDbContextFunc = rateDbContextFunc;
        this.basisIncomeTaxCalculatorFunc = basisIncomeTaxCalculatorFunc;
        this.mapper = mapper;
    }

    public async Task<Either<string, SingleTaxResult>> CalculateAsync(
        int calculationYear,
        int municipalityId,
        Canton canton,
        TaxPerson person)
    {
        var validationResult = await taxPersonValidator.ValidateAsync(person);
        if (!validationResult.IsValid)
        {
            var errorMessageLine = string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
            return $"validation failed: {errorMessageLine}";
        }

        var basisPerson = mapper.Map<BasisTaxPerson>(person);

        var incomeTaxResult =
            await basisIncomeTaxCalculatorFunc(canton)
                .CalculateAsync(calculationYear, canton, basisPerson);

        return incomeTaxResult
            .Match<Either<string, SingleTaxResult>>(
                Right: r => CalculateIncomeTax(
                    calculationYear, municipalityId, r),
                Left: msg => msg);
    }

    private SingleTaxResult CalculateIncomeTax(
        int calculationYear,
        int municipalityId,
        BasisTaxResult basisTaxResult)
    {
        using var dbContext = rateDbContextFunc();
        var taxRate = dbContext.Rates.AsNoTracking()
            .Single(item => item.Year == calculationYear
                            && item.BfsId == municipalityId);

        return new SingleTaxResult
        {
            BasisTaxAmount = basisTaxResult,
            MunicipalityRate = taxRate.TaxRateMunicipality,
            CantonRate = taxRate.TaxRateCanton,
        };
    }
}
