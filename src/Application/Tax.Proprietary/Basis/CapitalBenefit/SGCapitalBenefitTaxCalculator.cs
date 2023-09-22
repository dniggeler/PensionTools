using Application.Tax.Proprietary.Abstractions;
using Application.Tax.Proprietary.Abstractions.Models;
using Application.Tax.Proprietary.Abstractions.Models.Person;
using AutoMapper;
using Domain.Enums;
using Domain.Models.Tax;
using FluentValidation;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Application.Tax.Proprietary.Basis.CapitalBenefit;

public class SGCapitalBenefitTaxCalculator : ICapitalBenefitTaxCalculator
{
    private readonly IStateTaxRateRepository stateTaxRateRepository;
    private readonly IMapper mapper;
    private readonly IValidator<CapitalBenefitTaxPerson> validator;
    private readonly IChurchTaxCalculator churchTaxCalculator;

    public SGCapitalBenefitTaxCalculator(
        IMapper mapper,
        IValidator<CapitalBenefitTaxPerson> validator,
        IChurchTaxCalculator churchTaxCalculator,
        IStateTaxRateRepository stateTaxRateRepository)
    {
        this.stateTaxRateRepository = stateTaxRateRepository;
        this.mapper = mapper;
        this.validator = validator;
        this.churchTaxCalculator = churchTaxCalculator;
    }

    /// <inheritdoc />
    public async Task<Either<string, CapitalBenefitTaxResult>> CalculateAsync(
        int calculationYear,
        int municipalityId,
        Canton canton,
        CapitalBenefitTaxPerson capitalBenefitTaxPerson)
    {
        const decimal taxRateForSingle = 2.2M / 100M;
        const decimal taxRateForMarried = 2.0M / 100M;

        var validationResult = validator.Validate(capitalBenefitTaxPerson);
        if (!validationResult.IsValid)
        {
            return
                string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
        }

        var taxRateEntity = stateTaxRateRepository.TaxRates(calculationYear, municipalityId);

        if (taxRateEntity == null)
        {
            return
                $"No tax rate available for municipality {municipalityId} and year {calculationYear}";
        }

        BasisTaxResult basisTaxResult = GetBasisCapitalBenefitTaxAmount(capitalBenefitTaxPerson);

        ChurchTaxPerson churchTaxPerson = mapper.Map<ChurchTaxPerson>(capitalBenefitTaxPerson);

        Either<string, ChurchTaxResult> churchTaxResult =
            await churchTaxCalculator.CalculateAsync(
                churchTaxPerson,
                taxRateEntity,
                new AggregatedBasisTaxResult
                {
                    IncomeTax = basisTaxResult,
                    WealthTax = new BasisTaxResult(),
                });

        return churchTaxResult.Map(Update);

        CapitalBenefitTaxResult Update(ChurchTaxResult churchResult)
        {
            return new CapitalBenefitTaxResult
            {
                BasisTax = basisTaxResult,
                ChurchTax = churchResult,
                CantonRate = taxRateEntity.TaxRateCanton,
                MunicipalityRate = taxRateEntity.TaxRateMunicipality,
            };
        }

        BasisTaxResult GetBasisCapitalBenefitTaxAmount(CapitalBenefitTaxPerson person)
        {
            var amount = Some(person.CivilStatus)
                .Match(
                    Some: status => status switch
                    {
                        CivilStatus.Single =>
                            capitalBenefitTaxPerson.TaxableCapitalBenefits * taxRateForSingle,
                        CivilStatus.Married =>
                            capitalBenefitTaxPerson.TaxableCapitalBenefits * taxRateForMarried,
                        _ => 0M,
                    },
                    None: () => 0);

            return new BasisTaxResult
            {
                DeterminingFactorTaxableAmount = amount,
                TaxAmount = amount,
            };
        }
    }
}
