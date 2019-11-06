namespace TaxCalculator
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentValidation;
    using LanguageExt;
    using PensionCoach.Tools.TaxCalculator.Abstractions;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

    /// <inheritdoc />
    public class ChurchTaxCalculator : IChurchTaxCalculator
    {
        private readonly IValidator<ChurchTaxPerson> churchTaxPersonValidator;
        private readonly IValidator<SingleTaxResult> taxResultValidator;

        public ChurchTaxCalculator(
            IValidator<ChurchTaxPerson> churchTaxPersonValidator,
            IValidator<SingleTaxResult> basisTaxResultValidator)
        {
            this.churchTaxPersonValidator = churchTaxPersonValidator;
            this.taxResultValidator = basisTaxResultValidator;
        }

        /// <inheritdoc />
        public Task<Either<string, ChurchTaxResult>> CalculateAsync(
            int calculationYear, ChurchTaxPerson person, SingleTaxResult taxResult)
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

            Either<string, ChurchTaxResult> churchTaxResult = new ChurchTaxResult
            {
                TaxAmount = 117M,
                TaxAmountPartner = 117M,
            };

            return Task.FromResult(churchTaxResult);
        }
    }
}