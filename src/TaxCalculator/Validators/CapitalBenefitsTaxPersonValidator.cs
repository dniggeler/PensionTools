using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Validators
{
    public class CapitalBenefitsTaxPersonValidator : AbstractValidator<CapitalBenefitTaxPerson>
    {
        private const string ValueMustNotBeNegative = "Value must not be negative";

        public CapitalBenefitsTaxPersonValidator()
        {
            Include(new TaxPersonBasicValidator());

            RuleFor(p => p.TaxableBenefits)
                .GreaterThanOrEqualTo(decimal.Zero)
                .WithMessage(ValueMustNotBeNegative);
        }
    }
}
