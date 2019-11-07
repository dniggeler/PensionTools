using System.Linq;
using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace TaxCalculator.Validators
{
    public class TaxPersonValidator : AbstractValidator<TaxPerson>
    {
        private const int MinSupportedYear = 2018;
        private readonly string[] _supportedCantons = {"ZH"};
        public TaxPersonValidator()
        {
            RuleFor(x => x.CivilStatus).Must(x => x.IsSome);
            RuleFor(x => x.ReligiousGroupType).Must(x => x.IsSome);
            RuleFor(x => x.Canton).Must(x => _supportedCantons.Contains(x));
        }
    }
}