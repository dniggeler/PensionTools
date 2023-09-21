using Application.Tax.Proprietary.Abstractions.Models.Person;
using FluentValidation;

namespace Application.Validators;

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
