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
            TaxPerson taxPerson = this.mapper.Map<TaxPerson>(capitalBenefitTaxPerson);
            var stateTaxResult = await this.stateTaxCalculator.CalculateAsync(calculationYear, taxPerson);

            return stateTaxResult;
        }
    }
}