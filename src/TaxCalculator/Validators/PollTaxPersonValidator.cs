using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Validators
{
    public class PollTaxPersonValidator : AbstractValidator<PollTaxPerson>
    {
        public PollTaxPersonValidator()
        {
            this.RuleFor(x => x.Name).NotNull().NotEmpty();
            this.RuleFor(x => x.CivilStatus).Must(x => x.IsSome);
        }
    }
}