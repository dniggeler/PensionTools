namespace TaxCalculator
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentValidation;
    using LanguageExt;
    using PensionCoach.Tools.TaxCalculator.Abstractions;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
    using Tax.Data;
    using Tax.Data.Abstractions.Models;

    public class ChurchTaxCalculator : IChurchTaxCalculator
    {
        private readonly IValidator<ChurchTaxPerson> churchTaxPersonValidator;
        private readonly IValidator<AggregatedBasisTaxResult> taxResultValidator;
        private readonly Func<TaxRateDbContext> taxRateContextFunc;

        public ChurchTaxCalculator(
            IValidator<ChurchTaxPerson> churchTaxPersonValidator,
            IValidator<AggregatedBasisTaxResult> basisTaxResultValidator,
            Func<TaxRateDbContext> taxRateContextFunc)
        {
            this.churchTaxPersonValidator = churchTaxPersonValidator;
            this.taxResultValidator = basisTaxResultValidator;
            this.taxRateContextFunc = taxRateContextFunc;
        }

        /// <inheritdoc />
        public Task<Either<string, ChurchTaxResult>> CalculateAsync(
            int calculationYear, ChurchTaxPerson person, AggregatedBasisTaxResult taxResult)
        {
            if (person == null)
            {
                return Task
                    .FromResult<Either<string, ChurchTaxResult>>(
                        $"validation failed: {nameof(person)} null");
            }

            if (taxResult == null)
            {
                return Task
                    .FromResult<Either<string, ChurchTaxResult>>(
                        $"validation failed: {nameof(taxResult)} null");
            }

            var validationResult = this.churchTaxPersonValidator.Validate(person);

            if (!validationResult.IsValid)
            {
                var errorMessageLine = string.Join(
                    ";", validationResult.Errors.Select(x => x.ErrorMessage));
                return Task
                    .FromResult<Either<string, ChurchTaxResult>>(
                        $"validation failed: {errorMessageLine}");
            }

            var validationResultForTax = this.taxResultValidator.Validate(taxResult);

            if (!validationResultForTax.IsValid)
            {
                var errorMessageLine = string.Join(
                    ";", validationResult.Errors.Select(x => x.ErrorMessage));
                return Task
                    .FromResult<Either<string, ChurchTaxResult>>(
                        $"validation failed: {errorMessageLine}");
            }

            return this.CalculateInternalAsync(calculationYear, person, taxResult);
        }

        private Task<Either<string, ChurchTaxResult>> CalculateInternalAsync(
            int calculationYear, ChurchTaxPerson person, AggregatedBasisTaxResult taxResult)
        {
            decimal splitFactor =
                from c in person.CivilStatus
                from r in person.ReligiousGroup
                select DetermineSplitFactor(c, r, person.ReligiousGroupPartner);

            using (var context = this.taxRateContextFunc())
            {
                TaxRateModel taxRateModel = context.Rates
                    .Single(item => item.Year == calculationYear
                                   && item.Canton == person.Canton
                                   && item.Municipality == person.Municipality);

                return person.ReligiousGroup
                    .Map(group => group switch
                    {
                        ReligiousGroupType.Roman => taxRateModel.RomanChurchTaxRate,
                        ReligiousGroupType.Protestant => taxRateModel.ProtestantChurchTaxRate,
                        ReligiousGroupType.Catholic => taxRateModel.CatholicChurchTaxRate,
                        _ => 0M
                    })
                    .Match<Either<string, ChurchTaxResult>>(
                        Some: rate => new ChurchTaxResult
                        {
                            TaxAmount = rate / 100M * taxResult.Total,
                            TaxAmountPartner = rate / 100M * taxResult.Total,
                        },
                        None: () => "Calculation failed")
                    .AsTask();
            }
        }

        private decimal DetermineSplitFactor(
            CivilStatus civilStatus,
            ReligiousGroupType religiousGroupType,
            Option<ReligiousGroupType> personReligiousGroupPartner)
        {
            decimal splitFactor = (civilStatus, religiousGroupType) switch
            {
                (CivilStatus.Undefined, _) => 1.0M,
                (CivilStatus.Single, _) => 1.0M,
                (CivilStatus.Married, ReligiousGroupType.Undefined) => 1.0M,
                _ => 0.5M
            };

            return splitFactor;
        }
    }
}