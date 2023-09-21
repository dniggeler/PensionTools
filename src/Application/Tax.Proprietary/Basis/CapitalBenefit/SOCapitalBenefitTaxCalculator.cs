using Application.Tax.Proprietary.Abstractions;
using Application.Tax.Proprietary.Abstractions.Models;
using AutoMapper;
using Domain.Enums;
using Domain.Models.Tax;
using FluentValidation;
using Infrastructure.Tax.Data;
using LanguageExt;

namespace Application.Tax.Proprietary.Basis.CapitalBenefit;

public class SOCapitalBenefitTaxCalculator : ICapitalBenefitTaxCalculator
{
    private const decimal ScaleFactor = 0.25M;

    private readonly IMapper mapper;
    private readonly IValidator<CapitalBenefitTaxPerson> validator;
    private readonly IStateTaxCalculator stateTaxCalculator;
    private readonly IChurchTaxCalculator churchTaxCalculator;
    private readonly Func<TaxRateDbContext> dbContext;

    public SOCapitalBenefitTaxCalculator(
        IMapper mapper,
        IValidator<CapitalBenefitTaxPerson> validator,
        IStateTaxCalculator stateTaxCalculator,
        IChurchTaxCalculator churchTaxCalculator,
        Func<TaxRateDbContext> dbContext)
    {
        this.mapper = mapper;
        this.validator = validator;
        this.stateTaxCalculator = stateTaxCalculator;
        this.churchTaxCalculator = churchTaxCalculator;
        this.dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<Either<string, CapitalBenefitTaxResult>> CalculateAsync(
        int calculationYear,
        int municipalityId,
        Canton canton,
        CapitalBenefitTaxPerson capitalBenefitTaxPerson)
    {
        var validationResult = validator.Validate(capitalBenefitTaxPerson);
        if (!validationResult.IsValid)
        {
            return
                string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
        }

        var stateTaxPerson = mapper.Map<TaxPerson>(capitalBenefitTaxPerson);
        stateTaxPerson.TaxableIncome = capitalBenefitTaxPerson.TaxableCapitalBenefits;

        var stateTaxResult = await stateTaxCalculator
            .CalculateAsync(calculationYear, municipalityId, Canton.SO, stateTaxPerson);

        return stateTaxResult.Map(Scale);

        static CapitalBenefitTaxResult Scale(StateTaxResult intermediateResult)
        {
            return new CapitalBenefitTaxResult
            {
                BasisTax = new BasisTaxResult
                {
                    DeterminingFactorTaxableAmount =
                        intermediateResult.BasisIncomeTax.DeterminingFactorTaxableAmount * ScaleFactor,
                    TaxAmount =
                        intermediateResult.BasisIncomeTax.TaxAmount * ScaleFactor,
                },
                ChurchTax = new ChurchTaxResult
                {
                    TaxAmount = intermediateResult.ChurchTax.TaxAmount.Match(
                        Some: r => r * ScaleFactor,
                        None: () => Option<decimal>.None),

                    TaxAmountPartner = intermediateResult.ChurchTax.TaxAmountPartner.Match(
                        Some: r => r * ScaleFactor,
                        None: () => Option<decimal>.None),
                },
                CantonRate = intermediateResult.CantonRate,
                MunicipalityRate = intermediateResult.MunicipalityRate,
            };
        }
    }
}
