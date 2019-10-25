﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Tax.Data;
using Tax.Data.Abstractions;
using Tax.Data.Abstractions.Models;


namespace TaxCalculator
{
    public class WealthTaxCalculator : IWealthTaxCalculator
    {
        private const int TaxTypeId = (int)TaxType.Wealth;

        private readonly IValidator<TaxPerson> _taxPersonValidator;
        private readonly Func<TaxRateDbContext> _rateDbContextFunc;
        private readonly ITaxTariffData _tariffData;

        public WealthTaxCalculator(IValidator<TaxPerson> taxPersonValidator,
            Func<TaxRateDbContext> rateDbContextFunc,
            ITaxTariffData tariffData)
        {
            _taxPersonValidator = taxPersonValidator;
            _rateDbContextFunc = rateDbContextFunc;
            _tariffData = tariffData;
        }

        public Task<Either<TaxResult, string>> CalculateAsync(TaxPerson person)
        {
            var validationResult = _taxPersonValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                var errorMessageLine = string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
                return Task.FromResult<Either<TaxResult,string>>($"validation failed: {errorMessageLine}");
            }

            var tariffItems =
                _tariffData.Get(new TaxFilterModel
                    {
                        Year = person.CalculationYear,
                        Canton = person.Canton,
                        
                    })
                    .OrderBy(item => item.TaxAmount);

            var tariffTypeId = Map(person.CivilStatus)
                .Match(Some: t => (int)t,
                    None: () => 0);

            var tariffMatch = tariffItems
                .Where(item => item.TariffType == tariffTypeId)
                .Where(item => item.TaxType == TaxTypeId)
                .Where(item => item.IncomeLevel <= person.TaxableWealth)
                .OrderByDescending(item => item.IncomeLevel)
                .First();

            Either<TaxResult, string> result = CalculateTax(person, tariffMatch);

            return Task.FromResult(result);
        }

        private TaxResult CalculateTax(TaxPerson person, TaxTariffModel tariff)
        {
            using (var dbContext = _rateDbContextFunc())
            {
                var taxRate = dbContext.Rates
                    .Single(item => item.Canton == person.Canton &&
                                    item.Year == person.CalculationYear &&
                                    item.Municipality == person.Municipality);

                var referenceTaxableAmount = person.TaxableWealth - person.TaxableWealth % tariff.IncomeIncrement;

                var incrementMultiplier = (referenceTaxableAmount - tariff.IncomeLevel) / tariff.IncomeIncrement;

                var baseTaxAmount = incrementMultiplier * tariff.TaxIncrement + tariff.TaxAmount;

                return new TaxResult
                {
                    CalculationYear = person.CalculationYear,
                    ReferencedTaxableAmount = referenceTaxableAmount,
                    BaseTaxAmount = baseTaxAmount,
                    MunicipalityRate = taxRate.TaxRateMunicipality,
                    CantonRate = taxRate.TaxRateCanton,
                };
            }
        }

        private Option<TariffType> Map(Option<CivilStatus> status)
        {
            return status.Match(
                Some: s => s switch
                {
                    CivilStatus.Undefined => Option<TariffType>.None,
                    CivilStatus.Single => TariffType.Base,
                    CivilStatus.Married => TariffType.Married,
                    _ => Option<TariffType>.None
                },
                None: () => Option<TariffType>.None);
        }
    }
}
