using System.Linq;
using FluentValidation;
using PensionCoach.Tools.CommonTypes;

namespace PensionCoach.Tools.TaxCalculator.Validators
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
