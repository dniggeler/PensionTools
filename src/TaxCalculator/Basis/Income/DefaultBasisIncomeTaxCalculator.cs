using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Tax.Data.Abstractions;
using Tax.Data.Abstractions.Models;

namespace TaxCalculator.Basis.Income
{
    /// <summary>
    /// Default income tax calculator which is the same for ZH.
    /// </summary>
    /// <seealso cref="PensionCoach.Tools.TaxCalculator.Abstractions.IBasisIncomeTaxCalculator" />
    public class DefaultBasisIncomeTaxCalculator : IDefaultBasisIncomeTaxCalculator
    {
        private const int IncomeTaxTypeId = (int)TaxType.Income;

        private readonly IValidator<BasisTaxPerson> taxPersonValidator;
        private readonly ITaxTariffData tariffData;

        public DefaultBasisIncomeTaxCalculator(
            IValidator<BasisTaxPerson> taxPersonValidator,
            ITaxTariffData tariffData)
        {
            this.taxPersonValidator = taxPersonValidator;
            this.tariffData = tariffData;
        }

        public Task<Either<string, BasisTaxResult>> CalculateAsync(
            int calculationYear, Canton canton, BasisTaxPerson person)
        {
            Option<ValidationResult> validationResult =
                taxPersonValidator.Validate(person);

            var tariffType = validationResult
                .Where(v => !v.IsValid)
                .Map<Either<string, bool>>(r => string.Join(";", r.Errors.Select(x => x.ErrorMessage)))
                .IfNone(true)
                .Bind(_ => Map(person.CivilStatus));

            var tariffItems =
                tariffData.Get(new TaxFilterModel
                    {
                        Year = calculationYear,
                        Canton = canton.ToString(),
                    })
                    .OrderBy(item => item.TaxAmount);

            return tariffType

                // get all income level candidate
                .Map(typeId => tariffItems
                    .Where(item => item.TariffType == (int)typeId)
                    .Where(item => item.TaxType == IncomeTaxTypeId)
                    .Where(item => item.IncomeLevel <= person.TaxableAmount)
                    .OrderByDescending(item => item.IncomeLevel)
                    .DefaultIfEmpty(new TaxTariffModel())
                    .First())

                // calculate result
                .Map(tariff => CalculateIncomeTax(person, tariff))
                .AsTask();
        }

        private BasisTaxResult CalculateIncomeTax(BasisTaxPerson person, TaxTariffModel tariff)
        {
            var referenceTaxableIncome = person.TaxableAmount - (person.TaxableAmount % tariff.IncomeIncrement);

            var incrementMultiplier = referenceTaxableIncome - tariff.IncomeLevel;

            var baseTaxAmount =
                (incrementMultiplier * tariff.TaxTariffRatePercent / 100M) + tariff.TaxAmount;

            return new BasisTaxResult
            {
                DeterminingFactorTaxableAmount = referenceTaxableIncome,
                TaxAmount = baseTaxAmount,
            };
        }

        private Either<string, TariffType> Map(Option<CivilStatus> status)
        {
            return status.Match(
                Some: s => s switch
                {
                    CivilStatus.Undefined => Option<TariffType>.None,
                    CivilStatus.Single => TariffType.Base,
                    CivilStatus.Married => TariffType.Married,
                    _ => Option<TariffType>.None
                },
                None: () => Option<TariffType>.None)
                .ToEither("Civil status unknown");
        }
    }
}
