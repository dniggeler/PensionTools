using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Validators
{
    public class TaxPersonValidator : AbstractValidator<TaxPerson>
    {
        public TaxPersonValidator()
        {
            RuleFor(x => x.CivilStatus).Must(x => x.IsSome);
        }
    }
}
