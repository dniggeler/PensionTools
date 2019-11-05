namespace TaxCalculator.Validators
{
    using FluentValidation;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

    /// <inheritdoc />
    public class ChurchTaxResultValidator : AbstractValidator<SingleTaxResult>
    {
        public ChurchTaxResultValidator()
        {
            RuleFor(x => x.CantonRate).Must(x => x > 0M);
            RuleFor(x => x.CantonTaxAmount).Must(x => x >= 0M);
        }
    }
}