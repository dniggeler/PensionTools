using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Validators
{
    public class BasisTaxPersonValidator : AbstractValidator<BasisTaxPerson>
    {
        private const string ValueMustNotBeNegative = "Value must not be negative";

        public BasisTaxPersonValidator()
        {
            Include(new TaxPersonBasicValidator());
            RuleFor(p => p.TaxableAmount)
                .GreaterThanOrEqualTo(decimal.Zero)
                .WithMessage(ValueMustNotBeNegative);
        }
    }
}
