using System.Threading.Tasks;
using AutoMapper;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator
{
    public class FullTaxCalculator : IFullTaxCalculator
    {
        private readonly IMapper mapper;
        private readonly IStateTaxCalculator stateTaxCalculator;
        private readonly IFederalTaxCalculator federalTaxCalculator;

        public FullTaxCalculator(
            IMapper mapper,
            IStateTaxCalculator stateTaxCalculator,
            IFederalTaxCalculator federalTaxCalculator)
        {
            this.mapper = mapper;
            this.stateTaxCalculator = stateTaxCalculator;
            this.federalTaxCalculator = federalTaxCalculator;
        }

        public async Task<Either<string, FullTaxResult>> CalculateAsync(
            int calculationYear,
            int municipalityId,
            Canton canton,
            TaxPerson person)
        {
            var federalTaxPerson = mapper.Map<FederalTaxPerson>(person);

            var stateTaxResultTask =
                stateTaxCalculator
                    .CalculateAsync(calculationYear, municipalityId, canton, person);
            var federalTaxResultTask =
                federalTaxCalculator.CalculateAsync(calculationYear, federalTaxPerson);

            await Task.WhenAll(stateTaxResultTask, federalTaxResultTask);

            Either<string, StateTaxResult> stateTaxResult = await stateTaxResultTask;
            Either<string, BasisTaxResult> federalTaxResult = await federalTaxResultTask;

            var fullResult = new FullTaxResult();

            return stateTaxResult
                .Bind(r =>
                {
                    fullResult.StateTaxResult = r;
                    return federalTaxResult;
                })
                .Map(r =>
                {
                    fullResult.FederalTaxResult = r;
                    return fullResult;
                });
        }
    }
}