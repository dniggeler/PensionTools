using System.Linq;
using FluentValidation;

namespace TaxCalculator.Validators
{
    public class CantonIncomeTaxValidator : AbstractValidator<string>
    {
        private readonly string[] supportedCantons = { "ZH" };

        public CantonIncomeTaxValidator()
        {
            this.RuleFor(x => x).NotEmpty();
            this.RuleFor(x => x).NotNull();
            this.RuleFor(c => c).Must(c => this.supportedCantons.Contains(c));
        }
    }
}