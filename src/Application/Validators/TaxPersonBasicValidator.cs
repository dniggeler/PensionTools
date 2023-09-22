using Domain.Enums;
using Domain.Models.Tax;
using FluentValidation;

namespace Application.Validators
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
