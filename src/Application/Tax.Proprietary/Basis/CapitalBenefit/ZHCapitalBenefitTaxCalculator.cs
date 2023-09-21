using Application.Tax.Proprietary.Abstractions;
using Application.Tax.Proprietary.Abstractions.Models;
using AutoMapper;
using Domain.Enums;
using Domain.Models.Tax;
using FluentValidation;
using LanguageExt;

namespace Application.Tax.Proprietary.Basis.CapitalBenefit;

public class ZHCapitalBenefitTaxCalculator : ICapitalBenefitTaxCalculator
{
    private readonly IStateTaxCalculator stateTaxCalculator;
    private readonly IValidator<CapitalBenefitTaxPerson> validator;
    private readonly IMapper mapper;

    public ZHCapitalBenefitTaxCalculator(
        IStateTaxCalculator stateTaxCalculator,
        IValidator<CapitalBenefitTaxPerson> validator,
        IMapper mapper)
    {
        this.stateTaxCalculator = stateTaxCalculator;
        this.validator = validator;
        this.mapper = mapper;
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
            var errorMessageLine =
                string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));

            return errorMessageLine;
        }

        const decimal annuitizeFactor = 10;
        TaxPerson taxPerson = mapper.Map<TaxPerson>(capitalBenefitTaxPerson);

        taxPerson.TaxableIncome = capitalBenefitTaxPerson.TaxableCapitalBenefits / annuitizeFactor;

        var stateTaxResult = await stateTaxCalculator.CalculateAsync(
            calculationYear,
            municipalityId,
            canton,
            taxPerson);

        return stateTaxResult
            .Map(r => Scale(r, annuitizeFactor));
    }

    private CapitalBenefitTaxResult Scale(StateTaxResult intermediateResult, decimal scaleFactor)
    {
        var result = new CapitalBenefitTaxResult
        {
            BasisTax = new BasisTaxResult
            {
                DeterminingFactorTaxableAmount =
                    intermediateResult.BasisIncomeTax.DeterminingFactorTaxableAmount * scaleFactor,
                TaxAmount =
                    intermediateResult.BasisIncomeTax.TaxAmount * scaleFactor,
            },
            ChurchTax = new ChurchTaxResult
            {
                TaxAmount = intermediateResult.ChurchTax.TaxAmount.Match(
                    Some: r => r * scaleFactor,
                    None: () => Option<decimal>.None),

                TaxAmountPartner = intermediateResult.ChurchTax.TaxAmountPartner.Match(
                    Some: r => r * scaleFactor,
                    None: () => Option<decimal>.None),
            },
            CantonRate = intermediateResult.CantonRate,
            MunicipalityRate = intermediateResult.MunicipalityRate,
        };

        return result;
    }
}
