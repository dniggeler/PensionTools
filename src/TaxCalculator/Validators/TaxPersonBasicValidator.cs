using FluentValidation;
using PensionCoach.Tools.CommonTypes;

namespace TaxCalculator.Validators
{
    public class TaxPersonBasicValidator : AbstractValidator<TaxPersonBasic>
    {
        public TaxPersonBasicValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();

            RuleFor(x => x.NumberOfChildren).GreaterThanOrEqualTo(0);

            RuleFor(x => x.CivilStatus).Must(x => x != CivilStatus.Undefined);
        }
    }
}
