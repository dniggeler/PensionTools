using Domain.Models.Tax.Person;
using FluentValidation;

namespace Application.Validators;

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
