using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Basis.CapitalBenefit
{
    public class SGCapitalBenefitTaxCalculator : ICapitalBenefitTaxCalculator
    {
        private readonly IValidator<CapitalBenefitTaxPerson> validator;
        private readonly IMapper mapper;

        public SGCapitalBenefitTaxCalculator(
            IValidator<CapitalBenefitTaxPerson> validator,
            IMapper mapper)
        {
            this.validator = validator;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<Either<string, CapitalBenefitTaxResult>> CalculateAsync(
            int calculationYear,
            int municipalityId,
            Canton canton,
            CapitalBenefitTaxPerson capitalBenefitTaxPerson)
        {
            const decimal taxRateForSingle = 2.2M / 100M;
            const decimal taxRateForMarried = 2.0M / 100M;

            var validationResult = this.validator.Validate(capitalBenefitTaxPerson);
            if (!validationResult.IsValid)
            {
                var errorMessageLine =
                    string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));

                return errorMessageLine;
            }

            Option<CapitalBenefitTaxResult> result = capitalBenefitTaxPerson.CivilStatus
                .Map(status => GetTaxAmount(status))
                .Map(v => new CapitalBenefitTaxResult
                {
                    BasisTax = new BasisTaxResult
                    {
                        DeterminingFactorTaxableAmount = capitalBenefitTaxPerson.TaxableBenefits,
                        TaxAmount = v,
                    },
                });

            return result
                .Match<Either<string, CapitalBenefitTaxResult>>(
                    Some: r => r,
                    None: () => "Calculation failed");

            decimal GetTaxAmount(CivilStatus status)
            {
                return status switch
                {
                    CivilStatus.Single =>
                    capitalBenefitTaxPerson.TaxableBenefits * taxRateForSingle,
                    CivilStatus.Married =>
                    capitalBenefitTaxPerson.TaxableBenefits * taxRateForMarried,
                    _ => 0M,
                };
            }
        }
    }
}