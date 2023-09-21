using Application.Tax.Proprietary.Abstractions.Models.Person;
using FluentValidation;

namespace Application.Validators;

/// <inheritdoc />
public class ChurchTaxPersonValidator : AbstractValidator<ChurchTaxPerson>
{
    public ChurchTaxPersonValidator()
    {
        Include(new TaxPersonBasicValidator());
    }
}
