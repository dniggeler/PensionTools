using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Validators
{
    public class CapitalBenefitsTaxPersonValidator : AbstractValidator<CapitalBenefitTaxPerson>
    {
        public CapitalBenefitsTaxPersonValidator()
        {
            this.RuleFor(x => x.CivilStatus).Must(x => x.IsSome);
            this.RuleFor(x => x.ReligiousGroupType).Must(x => x.IsSome);
        }
    }
}