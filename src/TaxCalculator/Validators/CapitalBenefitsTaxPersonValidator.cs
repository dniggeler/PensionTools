using FluentValidation;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Validators
{
    public class CapitalBenefitsTaxPersonValidator : AbstractValidator<CapitalBenefitTaxPerson>
    {
        public CapitalBenefitsTaxPersonValidator()
        {
            RuleFor(x => x.CivilStatus)
                .Must(x => x.IfNone(CivilStatus.Undefined) != CivilStatus.Undefined);
            RuleFor(x => x.ReligiousGroupType).Must(x => x.IsSome);
        }
    }
}