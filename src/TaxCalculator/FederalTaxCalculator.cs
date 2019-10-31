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
    public class FederalTaxCalculator : IFederalTaxCalculator
    {
        private const int IncomeTaxTypeId = (int)TaxType.Income;

        private readonly IValidator<BasisTaxPerson> _taxPersonValidator;
        private readonly FederalTaxTariffDbContext _federalDbContext;

        public FederalTaxCalculator(IValidator<BasisTaxPerson> taxPersonValidator, FederalTaxTariffDbContext federalDbContext)
        {
            _taxPersonValidator = taxPersonValidator;
            _federalDbContext = federalDbContext;
        }

        public Task<Either<string, BasisTaxResult>> CalculateAsync(int calculationYear, BasisTaxPerson person)
        {
            var validationResult = _taxPersonValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                var errorMessageLine = string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
                return Task.FromResult<Either<string, BasisTaxResult>>($"validation failed: {errorMessageLine}");
            }

            return Map(person.CivilStatus)
                // get all income level candidate
                .Map(typeId => _federalDbContext.Tariffs
                    .Where(item => item.Year == calculationYear)
                    .Where(item => item.TariffType == (int) typeId)
                    .Where(item => item.TaxType == IncomeTaxTypeId)
                    .Where(item => item.IncomeLevel <= person.TaxableAmount)
                    .OrderByDescending(item => item.IncomeLevel))
                // take the largest one
                .Map(items => items.First())
                // calculate result
                .Map(tariff => CalculateTax(person, tariff))
                .Match<Either<string, BasisTaxResult>>(
                    Some: r => r,
                    None: () => "Tariff not available")
                .AsTask();
        }

        private BasisTaxResult CalculateTax(BasisTaxPerson person, FederalTaxTariffModel tariff)
        {
            {
                var referenceTaxableIncome = person.TaxableAmount - person.TaxableAmount % tariff.IncomeIncrement;

                var incrementMultiplier = (referenceTaxableIncome - tariff.IncomeLevel) / tariff.IncomeIncrement;

                var baseTaxAmount = incrementMultiplier * tariff.TaxIncrement + tariff.TaxAmount;

                return new BasisTaxResult
                {
                    DeterminingFactorTaxableAmount = referenceTaxableIncome,
                    TaxAmount = baseTaxAmount,
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
