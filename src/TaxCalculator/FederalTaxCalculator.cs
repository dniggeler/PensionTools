using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Tax.Data;
using Tax.Data.Abstractions.Models;


namespace TaxCalculator
{
    public class FederalTaxCalculator : IFederalTaxCalculator
    {
        private readonly IValidator<FederalTaxPerson> _taxPersonValidator;
        private readonly Func<FederalTaxTariffDbContext> _federalDbContext;

        public FederalTaxCalculator(IValidator<FederalTaxPerson> taxPersonValidator,
            Func<FederalTaxTariffDbContext> federalDbContext)
        {
            _taxPersonValidator = taxPersonValidator;
            _federalDbContext = federalDbContext;
        }

        public Task<Either<string, BasisTaxResult>> CalculateAsync(int calculationYear, FederalTaxPerson person)
        {
            var validationResult = _taxPersonValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                var errorMessageLine = string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
                return Task.FromResult<Either<string, BasisTaxResult>>($"validation failed: {errorMessageLine}");
            }

            using (var ctxt = _federalDbContext())
            {
                return Map(person.CivilStatus)
                    // get all income level candidate
                    .Map(typeId => ctxt.Tariffs
                        .Where(item => item.Year == calculationYear)
                        .Where(item => item.TariffType == (int)typeId)
                        .ToList()
                        .Where(item => item.IncomeLevel <= person.TaxableIncome)
                        .OrderByDescending(item => item.IncomeLevel)
                        .DefaultIfEmpty(new FederalTaxTariffModel())
                        .First())
                    // calculate result
                    .Map(tariff => this.CalculateTax(person, tariff))
                    .Match<Either<string, BasisTaxResult>>(
                        Some: r => r,
                        None: () => "Tariff not available")
                    .AsTask();
            }
        }

        private BasisTaxResult CalculateTax(FederalTaxPerson person, FederalTaxTariffModel tariff)
        {
            var referenceTaxableIncome = person.TaxableIncome - person.TaxableIncome % tariff.IncomeIncrement;

            var incrementMultiplier = (referenceTaxableIncome - tariff.IncomeLevel) / tariff.IncomeIncrement;

            var baseTaxAmount = incrementMultiplier * tariff.TaxIncrement + tariff.TaxAmount;

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
