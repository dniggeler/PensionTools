namespace TaxCalculator.Validators
{
    using FluentValidation;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

    /// <inheritdoc />
    public class AggregatedTaxResultValidator : AbstractValidator<AggregatedBasisTaxResult>
    {
        public AggregatedTaxResultValidator()
        {
            this.RuleFor(x => x.IncomeTax).NotNull();
            this.RuleFor(x => x.WealthTax).NotNull();
        }
    }
}