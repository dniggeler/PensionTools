using Domain.Enums;
using Domain.Models.Bvg;
using FluentValidation;

namespace Application.Validators
{
    public class BvgPersonValidator : AbstractValidator<BvgPerson>
    {
        public BvgPersonValidator()
        {
            RuleFor(x => x.Gender).Must(x => x != Gender.Undefined);
            RuleFor(x => x.PartTimeDegree).Must(x => x > decimal.Zero);
        }
    }
}
