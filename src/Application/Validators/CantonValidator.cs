using Domain.Enums;
using FluentValidation;

namespace Application.Validators
{
    public class CantonValidator : AbstractValidator<Canton>
    {
        private static readonly Canton[] SupportedCantons = { Canton.ZH, Canton.SG, Canton.SO };

        public CantonValidator()
        {
            RuleFor(canton => canton)
                .Must(c => SupportedCantons.Contains(c))
                .WithMessage(c => $"Canton {c} is not yet supported");
        }
    }
}
