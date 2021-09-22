using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Validators
{
    public class BasisTaxPersonValidator : AbstractValidator<BasisTaxPerson>
    {
        private const string ValueMustNotBeNegative = "Value must not be negative";

        public BasisTaxPersonValidator()
        {
            RuleFor(p => p.TaxableAmount)
                .Must(value => value >= decimal.Zero)
                .WithMessage(ValueMustNotBeNegative);
        }
    }
}
