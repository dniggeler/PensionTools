using Domain.Enums;
using FluentValidation;
using PensionCoach.Tools.BvgCalculator.Models;
using PensionCoach.Tools.CommonTypes;

namespace PensionCoach.Tools.BvgCalculator.Validators
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
