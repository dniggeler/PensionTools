using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentValidation;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Tax.Data;
using Tax.Data.Abstractions.Models;


namespace TaxCalculator
{
    public class IncomeTaxCalculator : IIncomeTaxCalculator
    {
        private readonly IValidator<TaxPerson> _taxPersonValidator;
        private readonly Func<TaxRateDbContext> _rateDbContextFunc;
        private readonly Func<TaxTariffDbContext> _tariffDbContextFunc;

        public IncomeTaxCalculator(IValidator<TaxPerson> taxPersonValidator,
            Func<TaxRateDbContext> rateDbContextFunc,
            Func<TaxTariffDbContext> tariffDbContextFunc)
        {
            _taxPersonValidator = taxPersonValidator;
            _rateDbContextFunc = rateDbContextFunc;
            _tariffDbContextFunc = tariffDbContextFunc;
        }

        public Task<Either<TaxResult, string>> CalculateAsync(TaxPerson person)
        {
            var validationResult = _taxPersonValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                var errorMessageLine = string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
                return Task.FromResult<Either<TaxResult,string>>($"validation failed: {errorMessageLine}");
            }

            using (var dbContext = _tariffDbContextFunc())
            {
                var tariffItems = dbContext.Tariffs
                    .Where(item => item.Year == person.CalculationYear &&
                                   item.Canton == person.Canton && 
                                   item.TariffType == 1 && 
                                   item.TaxType == 1)
                    .OrderBy( item => item.TaxAmount)
                    .ToList();

                var taxTariff = TariffMatch(tariffItems, person.TaxableIncome);
            }

            Option<TaxResult> result = from i in person.TaxableIncome
                select (new TaxResult
                {
                    TaxableIncome = i,
                    Rate = 0.02M,
                });
                    
            return Task.FromResult( result
                .Match<Either<TaxResult,string>>(
                    Some: r => r,
                    None: () => "incomplete input data"));
        }

        private Option<TaxTariffModel> TariffMatch(IEnumerable<TaxTariffModel> tariffItems, Option<decimal> personTaxableIncome)
        {
            foreach (var taxTariffModel in tariffItems)
            {
                if (taxTariffModel.TaxAmount >= personTaxableIncome)
                {
                    return taxTariffModel;
                }
            }

            return Option<TaxTariffModel>.None;
        }
    }
}
