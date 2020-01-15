using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Tax.Data;
using Tax.Data.Abstractions.Models;

namespace TaxCalculator.Basis.CapitalBenefit
{
    public class SGCapitalBenefitTaxCalculator : ICapitalBenefitTaxCalculator
    {
        private readonly IValidator<CapitalBenefitTaxPerson> validator;
        private readonly TaxRateDbContext dbContext;

        public SGCapitalBenefitTaxCalculator(
            IValidator<CapitalBenefitTaxPerson> validator,
            TaxRateDbContext dbContext)
        {
            this.validator = validator;
            this.dbContext = dbContext;
        }

        /// <inheritdoc />
        public Task<Either<string, CapitalBenefitTaxResult>> CalculateAsync(
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
                Either<string, CapitalBenefitTaxResult> errorMessageLine =
                    string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));

                return errorMessageLine.AsTask();
            }

            var taxRateEntity = this.dbContext.Rates
                .FirstOrDefault(item => item.BfsId == municipalityId && item.Year == calculationYear);

            if (taxRateEntity == null)
            {
                Either<string, CapitalBenefitTaxResult> r =
                    $"No tax rate available for municipality { municipalityId} and year { calculationYear}";

                return r.AsTask();
            }

            Option<CapitalBenefitTaxResult> result =
                (from s in capitalBenefitTaxPerson.CivilStatus
                from r in capitalBenefitTaxPerson.ReligiousGroupType
                from rPartner in capitalBenefitTaxPerson.ReligiousGroupType
                select GetTaxAmount(s, r, rPartner, taxRateEntity))
                .Map(v => new CapitalBenefitTaxResult
                {
                    BasisTax = new BasisTaxResult
                    {
                        DeterminingFactorTaxableAmount = capitalBenefitTaxPerson.TaxableBenefits,
                        TaxAmount = v,
                    },
                    ChurchTax = new ChurchTaxResult
                    {
                        TaxAmount = 0M,
                        TaxAmountPartner = 0M,
                    },
                });

            return result
                .Match<Either<string, CapitalBenefitTaxResult>>(
                    Some: r => r,
                    None: () => "Calculation failed")
                .AsTask();

            decimal GetTaxAmount(
                CivilStatus status,
                ReligiousGroupType taxPerson,
                ReligiousGroupType partner,
                TaxRateEntity taxRateEntity)
            {
                return (taxRateEntity.TaxRateCanton + taxRateEntity.TaxRateMunicipality) / 100
                       * status switch
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