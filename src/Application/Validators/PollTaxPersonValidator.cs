using Domain.Models.Tax.Person;
using FluentValidation;

namespace Application.Validators;

public class PollTaxPersonValidator : AbstractValidator<PollTaxPerson>
{
    public PollTaxPersonValidator()
    {
        Include(new TaxPersonBasicValidator());
    }
}
