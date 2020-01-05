using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Tax.Data;

namespace TaxCalculator
{
    public class IncomeTaxCalculator : IIncomeTaxCalculator
    {
        private readonly IValidator<TaxPerson> taxPersonValidator;
        private readonly Func<TaxRateDbContext> rateDbContextFunc;
        private readonly IBasisIncomeTaxCalculator basisIncomeTaxCalculator;
        private readonly IMapper mapper;

        public IncomeTaxCalculator(
            IValidator<TaxPerson> taxPersonValidator,
            Func<TaxRateDbContext> rateDbContextFunc,
            IBasisIncomeTaxCalculator basisIncomeTaxCalculator,
            IMapper mapper)
        {
            this.taxPersonValidator = taxPersonValidator;
            this.rateDbContextFunc = rateDbContextFunc;
            this.basisIncomeTaxCalculator = basisIncomeTaxCalculator;
            this.mapper = mapper;
        }

        public async Task<Either<string, SingleTaxResult>> CalculateAsync(
            int calculationYear,
            int municipalityId,
            Canton canton,
            TaxPerson person)
        {
            var validationResult = this.taxPersonValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                var errorMessageLine = string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
                return $"validation failed: {errorMessageLine}";
            }

            var basisPerson = this.mapper.Map<BasisTaxPerson>(person);
            Either<string, BasisTaxResult> incomeTaxResult =
                await this.basisIncomeTaxCalculator.CalculateAsync(
                    calculationYear, canton, basisPerson);

            return incomeTaxResult
                .Match<Either<string, SingleTaxResult>>(
                    Right: r => this.CalculateIncomeTax(
                        calculationYear, municipalityId, person, r),
                    Left: msg => msg);
        }

        private SingleTaxResult CalculateIncomeTax(
            int calculationYear,
            int municipalityId,
            TaxPerson person,
            BasisTaxResult basisTaxResult)
        {
            using (var dbContext = this.rateDbContextFunc())
            {
                var taxRate = dbContext.Rates
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
    }
}
