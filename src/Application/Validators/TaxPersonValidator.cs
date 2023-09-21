using Domain.Models.Tax;
using FluentValidation;

namespace Application.Validators;

public class TaxPersonValidator : AbstractValidator<TaxPerson>
{
    private const string ValueMustNotBeNegative = "Value must not be negative";

    public TaxPersonValidator()
    {
        Include(new TaxPersonBasicValidator());

        RuleFor(p => p.TaxableIncome)
            .GreaterThanOrEqualTo(decimal.Zero)
            .WithMessage(ValueMustNotBeNegative);
        RuleFor(p => p.TaxableWealth)
            .GreaterThanOrEqualTo(decimal.Zero)
            .WithMessage(ValueMustNotBeNegative);
        RuleFor(p => p.TaxableFederalIncome)
            .GreaterThanOrEqualTo(decimal.Zero)
            .WithMessage(ValueMustNotBeNegative);
    }
}
