using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Validators
{
    public class TaxPersonValidator : AbstractValidator<TaxPerson>
    {
        private const string ValueMustNotBeNegative = "Value must not be negative";

        public TaxPersonValidator()
        {
            RuleFor(p => p.TaxableIncome)
                .Must(value => value >= decimal.Zero)
                .WithMessage(ValueMustNotBeNegative);
            RuleFor(p => p.TaxableWealth)
                .Must(value => value >= decimal.Zero)
                .WithMessage(ValueMustNotBeNegative);
            RuleFor(p => p.TaxableFederalIncome)
                .Must(value => value >= decimal.Zero)
                .WithMessage(ValueMustNotBeNegative);
        }
    }
}
