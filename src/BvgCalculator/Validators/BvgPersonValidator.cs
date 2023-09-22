using Application.Bvg.Models;
using Domain.Enums;
using FluentValidation;

namespace PensionCoach.Tools.BvgCalculator.Validators;

public class BvgPersonValidator : AbstractValidator<BvgPerson>
{
    public BvgPersonValidator()
    {
        RuleFor(x => x.Gender).Must(x => x != Gender.Undefined);
        RuleFor(x => x.PartTimeDegree).Must(x => x > decimal.Zero);
    }
}
