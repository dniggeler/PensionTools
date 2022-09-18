using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Tax.Data;
using Tax.Data.Abstractions.Models;

namespace PensionCoach.Tools.TaxCalculator.Proprietary
{
    public class ProprietaryFederalTaxCalculator : IFederalTaxCalculator
    {
        private readonly IValidator<FederalTaxPerson> taxPersonValidator;
        private readonly FederalTaxTariffDbContext federalDbContext;

        public ProprietaryFederalTaxCalculator(
            IValidator<FederalTaxPerson> taxPersonValidator,
            FederalTaxTariffDbContext federalDbContext)
        {
            this.taxPersonValidator = taxPersonValidator;
            this.federalDbContext = federalDbContext;
        }

        /// <inheritdoc/>
        public Task<Either<string, BasisTaxResult>> CalculateAsync(int calculationYear, FederalTaxPerson person)
        {
            Option<ValidationResult> validationResult = taxPersonValidator.Validate(person);

            return validationResult
                .Where(r => !r.IsValid)
                .Map<Either<string, bool>>(r =>
                {
                    var errorMessageLine = string.Join(";", r.Errors.Select(x => x.ErrorMessage));
                    return $"validation failed: {errorMessageLine}";
                })
                .IfNone(true)
                .Bind(_ => Map(person.CivilStatus))

                // get all income level candidate
                .Map(typeId => federalDbContext.Tariffs
                    .Where(item => item.Year == calculationYear)
                    .Where(item => item.TariffType == (int)typeId)
                    .ToList()
                    .Where(item => item.IncomeLevel <= person.TaxableAmount)
                    .OrderByDescending(item => item.IncomeLevel)
                    .DefaultIfEmpty(new FederalTaxTariffModel())
                    .First())

                // calculate result
                .Map(tariff => CalculateTax(person, tariff))
                .AsTask();
        }

        private BasisTaxResult CalculateTax(FederalTaxPerson person, FederalTaxTariffModel tariff)
        {
            var referenceTaxableIncome =
                person.TaxableAmount - person.TaxableAmount % tariff.IncomeIncrement;

            var incrementMultiplier = (referenceTaxableIncome - tariff.IncomeLevel) / tariff.IncomeIncrement;

            var baseTaxAmount = incrementMultiplier * tariff.TaxIncrement + tariff.TaxAmount;

            return new BasisTaxResult
            {
                DeterminingFactorTaxableAmount = referenceTaxableIncome,
                TaxAmount = baseTaxAmount,
            };
        }

        private Either<string, TariffType> Map(Option<CivilStatus> status)
        {
            return status.Match<Either<string, TariffType>>(
                Some: s => s switch
                {
                    CivilStatus.Single => TariffType.Base,
                    CivilStatus.Married => TariffType.Married,
                    _ => TariffType.Undefined
                },
                None: () => "Civil status unknown");
        }
    }
}
