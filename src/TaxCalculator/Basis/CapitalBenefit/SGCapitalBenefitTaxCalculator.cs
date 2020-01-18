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

            BasisTaxResult basisTaxResult = GetBasisCapitalBenefitTaxAmount(capitalBenefitTaxPerson);

            ChurchTaxPerson churchTaxPerson = this.mapper.Map<ChurchTaxPerson>(capitalBenefitTaxPerson);

            Either<string, ChurchTaxResult> churchTaxResult =
                await this.churchTaxCalculator.CalculateAsync(
                    churchTaxPerson,
                    taxRateEntity,
                    new AggregatedBasisTaxResult
                    {
                        IncomeTax = basisTaxResult,
                        WealthTax = new BasisTaxResult(),
                    });

            return churchTaxResult.Map(Update);

            CapitalBenefitTaxResult Update(ChurchTaxResult churchResult)
            {
                return new CapitalBenefitTaxResult
                {
                    BasisTax = basisTaxResult,
                    ChurchTax = churchResult,
                    CantonRate = taxRateEntity.TaxRateCanton,
                    MunicipalityRate = taxRateEntity.TaxRateMunicipality,
                };
            }

            BasisTaxResult GetBasisCapitalBenefitTaxAmount(CapitalBenefitTaxPerson person)
            {
                var amount = person.CivilStatus
                    .Match(
                        Some: status => status switch
                        {
                            CivilStatus.Single =>
                            capitalBenefitTaxPerson.TaxableBenefits * taxRateForSingle,
                            CivilStatus.Married =>
                            capitalBenefitTaxPerson.TaxableBenefits * taxRateForMarried,
                            _ => 0M,
                        },
                        None: () => 0);

                return new BasisTaxResult
                {
                    DeterminingFactorTaxableAmount = amount,
                    TaxAmount = amount,
                };
            }
        }
    }
}