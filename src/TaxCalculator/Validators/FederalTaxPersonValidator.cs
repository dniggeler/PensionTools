using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace TaxCalculator.Validators
{
    public class FederalTaxPersonValidator : AbstractValidator<FederalTaxPerson>
    {
        public FederalTaxPersonValidator()
        {
            this.RuleFor(x => x.Name).NotNull().NotEmpty();
            this.RuleFor(x => x.CivilStatus).Must(x => x.IsSome);
        }
    }
}