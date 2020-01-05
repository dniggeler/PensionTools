using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Validators
{
    /// <inheritdoc />
    public class ChurchTaxPersonValidator : AbstractValidator<ChurchTaxPerson>
    {
        public ChurchTaxPersonValidator()
        {
            this.RuleFor(x => x.Name).NotNull().NotEmpty();
            this.RuleFor(x => x.CivilStatus).Must(x => x.IsSome);
            this.RuleFor(x => x.ReligiousGroup).Must(x => x.IsSome);
        }
    }
}