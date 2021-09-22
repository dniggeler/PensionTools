using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Validators
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
