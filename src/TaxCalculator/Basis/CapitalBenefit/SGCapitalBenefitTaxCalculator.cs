using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper mapper;
        private readonly IValidator<CapitalBenefitTaxPerson> validator;
        private readonly IChurchTaxCalculator churchTaxCalculator;
        private readonly TaxRateDbContext dbContext;

        public SGCapitalBenefitTaxCalculator(
            IMapper mapper,
            IValidator<CapitalBenefitTaxPerson> validator,
            IChurchTaxCalculator churchTaxCalculator,
            TaxRateDbContext dbContext)
        {
            this.mapper = mapper;
            this.validator = validator;
            this.churchTaxCalculator = churchTaxCalculator;
            this.dbContext = dbContext;
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
                return
                    string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var taxRateEntity = this.dbContext.Rates
                .FirstOrDefault(item => item.BfsId == municipalityId && item.Year == calculationYear);

            if (taxRateEntity == null)
            {
                return
                    $"No tax rate available for municipality { municipalityId} and year { calculationYear}";
            }

            Either<string, CapitalBenefitTaxResult> capitalBenefitTaxResult =
                (from s in capitalBenefitTaxPerson.CivilStatus
                from r in capitalBenefitTaxPerson.ReligiousGroupType
                from rPartner in capitalBenefitTaxPerson.ReligiousGroupType
                select GetTaxAmount(s, r, rPartner))
                .Map(v => new CapitalBenefitTaxResult
                {
                    BasisTax = new BasisTaxResult
                    {
                        DeterminingFactorTaxableAmount = capitalBenefitTaxPerson.TaxableBenefits,
                        TaxAmount = v,
                    },
                    CantonRate = taxRateEntity.TaxRateCanton,
                    MunicipalityRate = taxRateEntity.TaxRateMunicipality,
                })
                .ToEither("Basis capital benefit calculation failed");

            var churchTaxPerson = this.mapper.Map<ChurchTaxPerson>(capitalBenefitTaxPerson);

            var churchTaxResult =
                await capitalBenefitTaxResult
                    .Map(r => new AggregatedBasisTaxResult
                    {
                        IncomeTax = r.BasisTax,
                        WealthTax = new BasisTaxResult(),
                    })
                    .MapAsync(r =>
                        this.churchTaxCalculator.CalculateAsync(churchTaxPerson, taxRateEntity, r));

            return
                from ct in churchTaxResult.Flatten()
                from ir in capitalBenefitTaxResult
                select Update(ir, ct);

            CapitalBenefitTaxResult Update(
                CapitalBenefitTaxResult benefitTaxResult,
                ChurchTaxResult churchResult)
            {
                benefitTaxResult.ChurchTax = churchResult;

                return benefitTaxResult;
            }

            decimal GetTaxAmount(
                CivilStatus status,
                ReligiousGroupType taxPerson,
                ReligiousGroupType partner)
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