using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace PensionCoach.Tools.TaxCalculator.Validators
{
    public class FederalTaxPersonValidator : AbstractValidator<FederalTaxPerson>
    {
        private const string ValueMustNotBeNegative = "Value must not be negative";

        public FederalTaxPersonValidator()
        {
            Include(new TaxPersonBasicValidator());

            RuleFor(p => p.TaxableAmount)
                .GreaterThanOrEqualTo(decimal.Zero)
                .WithMessage(ValueMustNotBeNegative);
        }
    }
}
