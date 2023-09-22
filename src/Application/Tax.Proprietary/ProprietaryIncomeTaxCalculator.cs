using Application.Tax.Proprietary.Abstractions;
using Application.Tax.Proprietary.Abstractions.Models;
using Application.Tax.Proprietary.Abstractions.Models.Person;
using Application.Tax.Proprietary.Abstractions.Repositories;
using AutoMapper;
using Domain.Enums;
using Domain.Models.Tax;
using FluentValidation;
using LanguageExt;

namespace Application.Tax.Proprietary
{
    public class ProprietaryIncomeTaxCalculator : IIncomeTaxCalculator
    {
        private readonly IValidator<TaxPerson> taxPersonValidator;
        private readonly IStateTaxRateRepository stateTaxRateRepository;
        private readonly Func<Canton, IBasisIncomeTaxCalculator> basisIncomeTaxCalculatorFunc;
        private readonly IMapper mapper;

        public ProprietaryIncomeTaxCalculator(
            IValidator<TaxPerson> taxPersonValidator,
            IStateTaxRateRepository stateTaxRateRepository,
            Func<Canton, IBasisIncomeTaxCalculator> basisIncomeTaxCalculatorFunc,
            IMapper mapper)
        {
            this.taxPersonValidator = taxPersonValidator;
            this.stateTaxRateRepository = stateTaxRateRepository;
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
            var taxRate = stateTaxRateRepository.TaxRates(calculationYear, municipalityId);

            return new SingleTaxResult
            {
                BasisTaxAmount = basisTaxResult,
                MunicipalityRate = taxRate.TaxRateMunicipality,
                CantonRate = taxRate.TaxRateCanton,
            };
        }
    }
}
