using System.Linq;
using FluentValidation;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace TaxCalculator.Validators
{
    public class CantonValidator : AbstractValidator<Canton>
    {
        private readonly Canton[] supportedCantons = { Canton.ZH };

        public CantonValidator()
        {
            RuleFor(canton => canton)
                .Must(c => supportedCantons.Contains(c))
                .WithMessage(c => $"Canton {c} is not yet supported");
        }
    }
}