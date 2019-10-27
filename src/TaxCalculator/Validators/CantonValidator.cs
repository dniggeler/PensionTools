using System.Linq;
using FluentValidation;

namespace TaxCalculator.Validators
{
    public class CantonValidator : AbstractValidator<string>
    {
        private readonly string[] _supportedCantons = { "ZH" };

        public CantonValidator()
        {
            RuleFor(x => x).NotEmpty();
            RuleFor(x => x).NotNull();
            RuleFor(c => c).Must(c => _supportedCantons.Contains(c));
        }
    }
}