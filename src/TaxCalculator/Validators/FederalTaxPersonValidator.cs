using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Validators
{
    public class FederalTaxPersonValidator : AbstractValidator<FederalTaxPerson>
    {
        public FederalTaxPersonValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.CivilStatus).Must(x => x.IsSome);
        }
    }
}