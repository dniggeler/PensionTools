namespace TaxCalculator.Validators
{
    using System.Linq;
    using FluentValidation;

    public class CantonCapitalBenefitsValidator : AbstractValidator<string>
    {
        private readonly string[] supportedCantons = { "ZH" };

        public CantonCapitalBenefitsValidator()
        {
            this.RuleFor(canton => canton)
                .Must(c => this.supportedCantons.Contains(c))
                .WithMessage(c => $"Canton {c} is not yet supported");
        }
    }
}