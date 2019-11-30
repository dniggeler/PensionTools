using System.Linq;
using FluentValidation;

namespace TaxCalculator.Validators
{
    public class CantonValidator : AbstractValidator<string>
    {
        private readonly string[] supportedCantons = { "ZH" };

        public CantonValidator()
        {
            this.RuleFor(canton => canton)
                .Must(c => this.supportedCantons.Contains(c))
                .WithMessage(c => $"Canton {c} is not yet supported");
        }
    }
}