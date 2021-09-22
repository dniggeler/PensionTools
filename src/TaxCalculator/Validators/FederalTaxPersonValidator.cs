using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Validators
{
    public class FederalTaxPersonValidator : AbstractValidator<FederalTaxPerson>
    {
        public FederalTaxPersonValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
        }
    }
}
