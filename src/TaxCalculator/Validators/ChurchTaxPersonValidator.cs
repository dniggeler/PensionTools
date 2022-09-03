using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace PensionCoach.Tools.TaxCalculator.Validators
{
    /// <inheritdoc />
    public class ChurchTaxPersonValidator : AbstractValidator<ChurchTaxPerson>
    {
        public ChurchTaxPersonValidator()
        {
            Include(new TaxPersonBasicValidator());
        }
    }
}
