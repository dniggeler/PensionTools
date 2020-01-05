using System.ComponentModel;
using System.Text;

namespace TaxCalculator
{
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentValidation;
    using LanguageExt;
    using PensionCoach.Tools.TaxCalculator.Abstractions;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

    public class FullCapitalBenefitTaxCalculator : IFullCapitalBenefitTaxCalculator
    {
        private readonly ICapitalBenefitTaxCalculator stateCalculator;
        private readonly IFederalCapitalBenefitTaxCalculator federalCalculator;
        private readonly IValidator<CapitalBenefitTaxPerson> validator;
        private readonly IMapper mapper;

        public FullCapitalBenefitTaxCalculator(
            ICapitalBenefitTaxCalculator stateCalculator,
            IFederalCapitalBenefitTaxCalculator federalCalculator,
            IValidator<CapitalBenefitTaxPerson> validator,
            IMapper mapper)
        {
            this.stateCalculator = stateCalculator;
            this.federalCalculator = federalCalculator;
            this.validator = validator;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<Either<string, FullCapitalBenefitTaxResult>> CalculateAsync(
            int calculationYear,
            int municipalityId,
            Canton canton,
            CapitalBenefitTaxPerson capitalBenefitTaxPerson)
        {
            var stateTaxResultTask =
                this.stateCalculator.CalculateAsync(
                    calculationYear, municipalityId, canton, capitalBenefitTaxPerson);

            var federalTaxPerson =
                this.mapper.Map<FederalTaxPerson>(capitalBenefitTaxPerson);

            var federalTaxResultTask =
                this.federalCalculator.CalculateAsync(calculationYear, federalTaxPerson);

            await Task.WhenAll(stateTaxResultTask, federalTaxResultTask);

            StringBuilder sb = new StringBuilder();

            Either<string, CapitalBenefitTaxResult> stateTaxResult = await stateTaxResultTask;
            Either<string, BasisTaxResult> federalTaxResult = await federalTaxResultTask;

            stateTaxResult.MapLeft(r => sb.AppendLine(r));
            federalTaxResult.MapLeft(r => sb.AppendLine(r));

            Option<FullCapitalBenefitTaxResult> fullResult =
                from s in stateTaxResult.ToOption()
                from f in federalTaxResult.ToOption()
                select new FullCapitalBenefitTaxResult
                {
                    StateResult = s,
                    FederalResult = f,
                };

            return fullResult
                .Match<Either<string, FullCapitalBenefitTaxResult>>(
                    Some: r => r,
                    None: () => sb.ToString());
        }

        private CapitalBenefitTaxResult Scale(StateTaxResult intermediateResult, decimal scaleFactor)
        {
            var result = new CapitalBenefitTaxResult
            {
                BasisTax = new BasisTaxResult
                {
                    DeterminingFactorTaxableAmount =
                        intermediateResult.BasisIncomeTax.DeterminingFactorTaxableAmount * scaleFactor,
                    TaxAmount =
                        intermediateResult.BasisIncomeTax.TaxAmount * scaleFactor,
                },
                ChurchTax = new ChurchTaxResult
                {
                    TaxAmount = intermediateResult.ChurchTax.TaxAmount.Match(
                        Some: r => r * scaleFactor,
                        None: () => Option<decimal>.None),

                    TaxAmountPartner = intermediateResult.ChurchTax.TaxAmountPartner.Match(
                        Some: r => r * scaleFactor,
                        None: () => Option<decimal>.None),
                },
                CantonRate = intermediateResult.CantonRate,
                MunicipalityRate = intermediateResult.MunicipalityRate,
            };

            return result;
        }
    }
}