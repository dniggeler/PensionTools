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
    public class WealthTaxCalculator : IWealthTaxCalculator
    {
        private readonly IValidator<TaxPerson> taxPersonValidator;
        private readonly Func<TaxRateDbContext> rateDbContextFunc;
        private readonly IBasisWealthTaxCalculator basisWealthTaxCalculator;
        private readonly IMapper mapper;

        public WealthTaxCalculator(
            IValidator<TaxPerson> taxPersonValidator,
            Func<TaxRateDbContext> rateDbContextFunc,
            IBasisWealthTaxCalculator basisWealthTaxCalculator,
            IMapper mapper)
        {
            this.taxPersonValidator = taxPersonValidator;
            this.rateDbContextFunc = rateDbContextFunc;
            this.basisWealthTaxCalculator = basisWealthTaxCalculator;
            this.mapper = mapper;
        }

        public async Task<Either<string, SingleTaxResult>> CalculateAsync(
            int calculationYear, int municipalityId, Canton canton, TaxPerson person)
        {
            var validationResult = this.taxPersonValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                var errorMessageLine = string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
                return $"validation failed: {errorMessageLine}";
            }

            var basisTaxPerson = this.mapper.Map<BasisTaxPerson>(person);

            basisTaxPerson.TaxableAmount = person.TaxableWealth;

            var basisTaxResult =
                await this.basisWealthTaxCalculator.CalculateAsync(
                    calculationYear, canton, basisTaxPerson);

            return basisTaxResult
                .Match<Either<string, SingleTaxResult>>(
                    Right: r => this.CalculateTax(calculationYear, municipalityId, r),
                    Left: msg => msg);
        }

        private SingleTaxResult CalculateTax(
            int calculationYear, int municipalityId, BasisTaxResult basisTaxResult)
        {
            using (var dbContext = this.rateDbContextFunc())
            {
                var taxRate = dbContext.Rates
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
    }
}
