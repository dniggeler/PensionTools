﻿using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Tax.Data.Abstractions;
using Tax.Data.Abstractions.Models;

namespace TaxCalculator.Basis.Wealth
{
    /// <summary>
    /// Wealth tax calculator for SO.
    /// </summary>
    /// <seealso cref="PensionCoach.Tools.TaxCalculator.Abstractions.IBasisWealthTaxCalculator" />
    public class SOBasisWealthTaxCalculator : IBasisWealthTaxCalculator
    {
        private const int TaxTypeId = (int)TaxType.Wealth;

        private readonly IValidator<BasisTaxPerson> taxPersonValidator;
        private readonly ITaxTariffData tariffData;

        public SOBasisWealthTaxCalculator(
            IValidator<BasisTaxPerson> taxPersonValidator,
            ITaxTariffData tariffData)
        {
            this.taxPersonValidator = taxPersonValidator;
            this.tariffData = tariffData;
        }

        public Task<Either<string, BasisTaxResult>> CalculateAsync(
            int calculationYear, Canton canton, BasisTaxPerson person)
        {
            var validationResult = this.taxPersonValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                var errorMessageLine = string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
                return Task.FromResult<Either<string, BasisTaxResult>>($"validation failed: {errorMessageLine}");
            }

            var tariffItems =
                this.tariffData.Get(new TaxFilterModel
                {
                    Year = calculationYear,
                    Canton = canton.ToString(),
                })
                    .OrderBy(item => item.TaxAmount);

            return this.Map(person.CivilStatus)

                // get all income level candidate
                .Map(typeId => tariffItems
                    .Where(item => item.TariffType == (int)TariffType.Base)
                    .Where(item => item.TaxType == TaxTypeId)
                    .Where(item => item.IncomeLevel <= person.TaxableAmount)
                    .OrderByDescending(item => item.IncomeLevel)
                    .DefaultIfEmpty(new TaxTariffModel())
                    .First())

                // calculate result
                .Map(tariff => this.CalculateIncomeTax(person, tariff))
                .Match<Either<string, BasisTaxResult>>(
                    Some: r => r,
                    None: () => "Tariff not available")
                .AsTask();
        }

        private BasisTaxResult CalculateIncomeTax(BasisTaxPerson person, TaxTariffModel tariff)
        {
            var referenceTaxableIncome =
                person.TaxableAmount - (person.TaxableAmount % tariff.IncomeIncrement);

            var incrementMultiplier = (referenceTaxableIncome - tariff.IncomeLevel) / tariff.IncomeIncrement;

            var baseTaxAmount = (incrementMultiplier * tariff.TaxTariffRatePercent / 1000m) + tariff.TaxAmount;

            return new BasisTaxResult
            {
                DeterminingFactorTaxableAmount = referenceTaxableIncome,
                TaxAmount = baseTaxAmount,
            };
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
