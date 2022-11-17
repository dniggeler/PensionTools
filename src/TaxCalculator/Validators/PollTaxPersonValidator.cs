using FluentValidation;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace PensionCoach.Tools.TaxCalculator.Validators
{
    public class PollTaxPersonValidator : AbstractValidator<PollTaxPerson>
    {
        public PollTaxPersonValidator()
        {
            Include(new TaxPersonBasicValidator());
        }
    }
}
