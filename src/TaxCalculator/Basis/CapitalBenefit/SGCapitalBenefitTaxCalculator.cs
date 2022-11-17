using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Tax.Data;
using static LanguageExt.Prelude;

namespace PensionCoach.Tools.TaxCalculator.Basis.CapitalBenefit
{
    public class SGCapitalBenefitTaxCalculator : ICapitalBenefitTaxCalculator
    {
        private readonly IMapper mapper;
        private readonly IValidator<CapitalBenefitTaxPerson> validator;
        private readonly IChurchTaxCalculator churchTaxCalculator;
        private readonly Func<TaxRateDbContext> dbContext;

        public SGCapitalBenefitTaxCalculator(
            IMapper mapper,
            IValidator<CapitalBenefitTaxPerson> validator,
            IChurchTaxCalculator churchTaxCalculator,
            Func<TaxRateDbContext> dbContext)
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

            var validationResult = validator.Validate(capitalBenefitTaxPerson);
            if (!validationResult.IsValid)
            {
                return
                    string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
            }

            await using var ctxt = dbContext();
            var taxRateEntity = ctxt.Rates.AsNoTracking()
                .FirstOrDefault(item => item.BfsId == municipalityId
                                        && item.Year == calculationYear);

            if (taxRateEntity == null)
            {
                return
                    $"No tax rate available for municipality {municipalityId} and year {calculationYear}";
            }

            BasisTaxResult basisTaxResult = GetBasisCapitalBenefitTaxAmount(capitalBenefitTaxPerson);

            ChurchTaxPerson churchTaxPerson = mapper.Map<ChurchTaxPerson>(capitalBenefitTaxPerson);

            Either<string, ChurchTaxResult> churchTaxResult =
                await churchTaxCalculator.CalculateAsync(
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
                var amount = Some(person.CivilStatus)
                    .Match(
                        Some: status => status switch
                        {
                            CivilStatus.Single =>
                            capitalBenefitTaxPerson.TaxableCapitalBenefits * taxRateForSingle,
                            CivilStatus.Married =>
                            capitalBenefitTaxPerson.TaxableCapitalBenefits * taxRateForMarried,
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
