using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Tax.Data;

namespace TaxCalculator
{
    public class WealthTaxCalculator : IWealthTaxCalculator
    {
        private readonly IValidator<TaxPerson> _taxPersonValidator;
        private readonly Func<TaxRateDbContext> _rateDbContextFunc;
        private readonly IBasisWealthTaxCalculator _basisWealthTaxCalculator;
        private readonly IMapper _mapper;

        public WealthTaxCalculator(IValidator<TaxPerson> taxPersonValidator,
            Func<TaxRateDbContext> rateDbContextFunc,
            IBasisWealthTaxCalculator basisWealthTaxCalculator,
            IMapper mapper)
        {
            _taxPersonValidator = taxPersonValidator;
            _rateDbContextFunc = rateDbContextFunc;
            _basisWealthTaxCalculator = basisWealthTaxCalculator;
            _mapper = mapper;
        }

        public async Task<Either<string, TaxResult>> CalculateAsync(int calculationYear, TaxPerson person)
        {
            var validationResult = _taxPersonValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                var errorMessageLine = string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
                return $"validation failed: {errorMessageLine}";
            }

            var basisTaxPerson = _mapper.Map<BasisTaxPerson>(person);

            basisTaxPerson.TaxableAmount = person.TaxableWealth;

            var basisTaxResult =
                await _basisWealthTaxCalculator.CalculateAsync(calculationYear, basisTaxPerson);

            return basisTaxResult
                .Match<Either<string, TaxResult>>(
                    Right: r => CalculateTax(calculationYear, person, r),
                    Left: msg => msg);
        }

        private TaxResult CalculateTax(int calculationYear, TaxPerson person, BasisTaxResult basisTaxResult)
        {
            using (var dbContext = _rateDbContextFunc())
            {
                var taxRate = dbContext.Rates
                    .Single(item => item.Canton == person.Canton &&
                                    item.Year == calculationYear &&
                                    item.Municipality == person.Municipality);

                return new TaxResult
                {
                    CalculationYear = calculationYear,
                    BasisWealthTax = basisTaxResult,
                    MunicipalityRate = taxRate.TaxRateMunicipality,
                    CantonRate = taxRate.TaxRateCanton,
                };
            }
        }
    }
}
