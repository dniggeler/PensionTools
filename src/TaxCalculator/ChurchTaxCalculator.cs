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
            var religiousGroupPartner = Prelude.Optional(
                person.PartnerReligiousGroup.IfNone(ReligiousGroupType.Other));

            var splitFactor =
                (from c in person.CivilStatus
                from r in person.ReligiousGroup
                from rp in religiousGroupPartner
                select this.DetermineSplitFactor(
                    c, r, rp))
                .IfNone(0M);

            using (var context = this.taxRateContextFunc())
            {
                TaxRateModel taxRateModel = context.Rates
                    .Single(item => item.Year == calculationYear
                                   && item.Canton == person.Canton
                                   && item.Municipality == person.Municipality);

                Option<ChurchTaxResult> result =
                        from ratePerson in person.ReligiousGroup.Map(GetTaxRate)
                        from ratePartner in religiousGroupPartner.Map(GetTaxRate)
                        select new ChurchTaxResult
                        {
                            TaxAmount = ratePerson / 100M * taxResult.Total * splitFactor,
                            TaxAmountPartner = ratePartner / 100M * taxResult.Total * (1M - splitFactor),
                        };

                return result.Match<Either<string, ChurchTaxResult>>(
                        Some: r => r,
                        None: () => "Calculation failed")
                    .AsTask();

                decimal GetTaxRate(ReligiousGroupType religiousGroupType)
                {
                    return religiousGroupType switch
                    {
                        ReligiousGroupType.Roman => taxRateModel.RomanChurchTaxRate,
                        ReligiousGroupType.Protestant => taxRateModel.ProtestantChurchTaxRate,
                        ReligiousGroupType.Catholic => taxRateModel.CatholicChurchTaxRate,
                        _ => 0M
                    };
                }
            }
        }

        private decimal DetermineSplitFactor(
            CivilStatus civilStatus,
            ReligiousGroupType religiousGroupType,
            ReligiousGroupType personReligiousGroupPartner)
        {
            decimal splitFactor = (civilStatus, religiousGroupType, personReligiousGroupPartner)
                switch
                {
                    (CivilStatus.Undefined, _, _) => 1.0M,
                    (CivilStatus.Single, ReligiousGroupType.Other, _) => 0.0M,
                    (CivilStatus.Single, _, _) => 1.0M,
                    (CivilStatus.Married, ReligiousGroupType.Other, ReligiousGroupType.Other)
                    => 0.0M,
                    (CivilStatus.Married, ReligiousGroupType.Other, _) => 1.0M,
                    _ => 0.5M
                };

            return splitFactor;
        }
    }
}