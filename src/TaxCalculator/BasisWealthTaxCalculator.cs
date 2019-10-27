using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Tax.Data.Abstractions;
using Tax.Data.Abstractions.Models;


namespace TaxCalculator
{
    public class BasisWealthTaxCalculator : IBasisWealthTaxCalculator
    {
        private const int TaxTypeId = (int)TaxType.Wealth;

        private readonly IValidator<BasisTaxPerson> _taxPersonValidator;
        private readonly ITaxTariffData _tariffData;

        public BasisWealthTaxCalculator(IValidator<BasisTaxPerson> taxPersonValidator,
            ITaxTariffData tariffData)
        {
            _taxPersonValidator = taxPersonValidator;
            _tariffData = tariffData;
        }

        public Task<Either<BasisTaxResult, string>> CalculateAsync(int calculationYear, BasisTaxPerson person)
        {
            var validationResult = _taxPersonValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                var errorMessageLine = string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
                return Task.FromResult<Either<BasisTaxResult, string>>($"validation failed: {errorMessageLine}");
            }

            var tariffItems =
                _tariffData.Get(new TaxFilterModel
                    {
                        Year = calculationYear,
                        Canton = person.Canton,

                    })
                    .OrderBy(item => item.TaxAmount);

            return Map(person.CivilStatus)
                // get all income level candidate
                .Map(typeId => tariffItems
                    .Where(item => item.TariffType == (int) typeId)
                    .Where(item => item.TaxType == TaxTypeId)
                    .Where(item => item.IncomeLevel <= person.TaxableAmount)
                    .OrderByDescending(item => item.IncomeLevel))
                // take the largest one
                .Map(items => items.First())
                // calculate result
                .Map(tariff => CalculateIncomeTax(person, tariff))
                .Match<Either<BasisTaxResult, string>>(
                    Some: r => r,
                    None: () => "Tariff not available")
                .AsTask();
        }

        private BasisTaxResult CalculateIncomeTax(BasisTaxPerson person, TaxTariffModel tariff)
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
