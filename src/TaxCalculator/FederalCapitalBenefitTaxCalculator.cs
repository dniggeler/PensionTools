namespace TaxCalculator
{
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentValidation;
    using LanguageExt;
    using PensionCoach.Tools.TaxCalculator.Abstractions;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

    public class FederalCapitalBenefitTaxCalculator : IFederalCapitalBenefitTaxCalculator
    {
        private readonly IFederalTaxCalculator taxCalculator;
        private readonly IValidator<CapitalBenefitTaxPerson> validator;
        private readonly IMapper mapper;

        public FederalCapitalBenefitTaxCalculator(
            IFederalTaxCalculator taxCalculator,
            IValidator<CapitalBenefitTaxPerson> validator,
            IMapper mapper)
        {
            this.taxCalculator = taxCalculator;
            this.validator = validator;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<Either<string, BasisTaxResult>> CalculateAsync(
            int calculationYear, FederalCapitalBenefitTaxPerson capitalBenefitTaxPerson)
        {
            const decimal scaleFactor = 0.2M;
            FederalTaxPerson taxPerson = this.mapper.Map<FederalTaxPerson>(capitalBenefitTaxPerson);

            var taxResult = await this.taxCalculator.CalculateAsync(calculationYear, taxPerson);

            return taxResult.Map(r => this.Scale(r, scaleFactor));
        }

        private BasisTaxResult Scale(BasisTaxResult intermediateResult, decimal scaleFactor)
        {
            return new BasisTaxResult
            {
                DeterminingFactorTaxableAmount =
                    intermediateResult.DeterminingFactorTaxableAmount,
                TaxAmount =
                    intermediateResult.TaxAmount * scaleFactor,
            };
        }
    }
}