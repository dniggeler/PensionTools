using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Validators
{
    public class BasisTaxPersonValidator : AbstractValidator<BasisTaxPerson>
    {
        public BasisTaxPersonValidator()
        {
            RuleFor(x => x.CivilStatus).Must(x => x.IsSome);
        }
    }
}
