namespace TaxCalculator.Validators
{
    using System.Linq;
    using FluentValidation;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

    public class CapitalBenefitsTaxPersonValidator : AbstractValidator<CapitalBenefitTaxPerson>
    {
        private readonly string[] supportedCantons = { "ZH" };

        public CapitalBenefitsTaxPersonValidator()
        {
            this.RuleFor(x => x.CivilStatus).Must(x => x.IsSome);
            this.RuleFor(x => x.ReligiousGroupType).Must(x => x.IsSome);
            this.RuleFor(x => x.Canton)
                .SetValidator(new CantonIncomeTaxValidator());
        }
    }
}