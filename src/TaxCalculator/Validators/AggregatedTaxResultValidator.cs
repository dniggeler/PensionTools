namespace TaxCalculator.Validators
{
    using FluentValidation;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

    /// <inheritdoc />
    public class AggregatedTaxResultValidator : AbstractValidator<AggregatedBasisTaxResult>
    {
        public AggregatedTaxResultValidator()
        {
            RuleFor(x => x.IncomeTax).NotNull();
            RuleFor(x => x.WealthTax).NotNull();
        }
    }
}
