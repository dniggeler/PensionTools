using Domain.Models.Tax;
using FluentValidation;
using PensionCoach.Tools.CommonTypes.Tax;

namespace PensionCoach.Tools.TaxCalculator.Validators
{
    public class CapitalBenefitsTaxPersonValidator : AbstractValidator<CapitalBenefitTaxPerson>
    {
        private const string ValueMustNotBeNegative = "Value must not be negative";

        public CapitalBenefitsTaxPersonValidator()
        {
            Include(new TaxPersonBasicValidator());

            RuleFor(p => p.TaxableCapitalBenefits)
                .GreaterThanOrEqualTo(decimal.Zero)
                .WithMessage(ValueMustNotBeNegative);
        }
    }
}
