using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace TaxCalculator.Validators
{
    public class BasisTaxPersonValidator : AbstractValidator<BasisTaxPerson>
    {
        
        public BasisTaxPersonValidator()
        {
            RuleFor(x => x.CivilStatus).Must(x => x.IsSome);
            RuleFor(x => x.Canton).SetValidator(new CantonValidator());
        }
    }
}