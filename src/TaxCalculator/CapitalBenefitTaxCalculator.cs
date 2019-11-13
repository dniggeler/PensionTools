using AutoMapper;
using FluentValidation;


namespace TaxCalculator
{
    using System.Threading.Tasks;
    using LanguageExt;
    using PensionCoach.Tools.TaxCalculator.Abstractions;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

    public class CapitalBenefitTaxCalculator : ICapitalBenefitTaxCalculator
    {
        private readonly IStateTaxCalculator stateTaxCalculator;
        private readonly IValidator<CapitalBenefitTaxPerson> validator;
        private readonly IMapper mapper;

        public CapitalBenefitTaxCalculator(
            IStateTaxCalculator stateTaxCalculator,
            IValidator<CapitalBenefitTaxPerson> validator,
            IMapper mapper)
        {
            this.stateTaxCalculator = stateTaxCalculator;
            this.validator = validator;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<Either<string, StateTaxResult>> CalculateAsync(
            int calculationYear, CapitalBenefitTaxPerson capitalBenefitTaxPerson)
        {
            const decimal annuitizeFactor = 10;
            TaxPerson taxPerson = this.mapper.Map<TaxPerson>(capitalBenefitTaxPerson);

            taxPerson.TaxableIncome = capitalBenefitTaxPerson.TaxableBenefits / annuitizeFactor;

            var stateTaxResult = await this.stateTaxCalculator.CalculateAsync(calculationYear, taxPerson);

            return stateTaxResult
                .Map(r => this.Scale(r, annuitizeFactor));
        }

        private StateTaxResult Scale(StateTaxResult intermediateResult, decimal scaleFactor)
        {
            intermediateResult.BasisIncomeTax.DeterminingFactorTaxableAmount
                = intermediateResult.BasisIncomeTax.DeterminingFactorTaxableAmount * scaleFactor;
            intermediateResult.BasisIncomeTax.TaxAmount
                = intermediateResult.BasisIncomeTax.TaxAmount * scaleFactor;

            intermediateResult.ChurchTax.TaxAmount =
                intermediateResult.ChurchTax.TaxAmount.Match(
                    Some: r => r * scaleFactor,
                    None: () => Option<decimal>.None);

            intermediateResult.ChurchTax.TaxAmountPartner =
                intermediateResult.ChurchTax.TaxAmountPartner.Match(
                    Some: r => r * scaleFactor,
                    None: () => Option<decimal>.None);

            intermediateResult.PollTaxAmount = Option<decimal>.None;

            return intermediateResult;
        }
    }
}