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
    public class IncomeTaxCalculator : IIncomeTaxCalculator
    {
        private readonly IValidator<TaxPerson> _taxPersonValidator;
        private readonly Func<TaxRateDbContext> _rateDbContextFunc;
        private readonly IBasisIncomeTaxCalculator _basisIncomeTaxCalculator;
        private readonly IMapper _mapper;

        public IncomeTaxCalculator(IValidator<TaxPerson> taxPersonValidator,
            Func<TaxRateDbContext> rateDbContextFunc, 
            IBasisIncomeTaxCalculator basisIncomeTaxCalculator,
            IMapper mapper)
        {
            _taxPersonValidator = taxPersonValidator;
            _rateDbContextFunc = rateDbContextFunc;
            _basisIncomeTaxCalculator = basisIncomeTaxCalculator;
            _mapper = mapper;
        }

        public async Task<Either<string,TaxResult>> CalculateAsync(int calculationYear, TaxPerson person)
        {
            var validationResult = _taxPersonValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                var errorMessageLine = string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
                return $"validation failed: {errorMessageLine}";
            }

            var basisPerson = _mapper.Map<BasisTaxPerson>(person);
            Either<string, BasisTaxResult> incomeTaxResult = 
                await _basisIncomeTaxCalculator.CalculateAsync(calculationYear, basisPerson);

            return incomeTaxResult
                .Match<Either<string,TaxResult>>(
                    Right: r => CalculateIncomeTax(calculationYear, person, r),
                    Left: msg => msg);
        }

        private TaxResult CalculateIncomeTax(int calculationYear, TaxPerson person, 
            BasisTaxResult basisTaxResult)
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
                    BasisIncomeTax = basisTaxResult,
                    MunicipalityRate = taxRate.TaxRateMunicipality,
                    CantonRate = taxRate.TaxRateCanton,
                };
            }
        }
    }
}
