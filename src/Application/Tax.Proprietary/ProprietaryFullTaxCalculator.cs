using Application.Tax.Proprietary.Abstractions;
using Application.Tax.Proprietary.Abstractions.Models.Person;
using AutoMapper;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Tax.Proprietary
{
    public class ProprietaryFullTaxCalculator : IFullWealthAndIncomeTaxCalculator
    {
        private readonly IMapper mapper;
        private readonly IStateTaxCalculator stateTaxCalculator;
        private readonly IFederalTaxCalculator federalTaxCalculator;

        public ProprietaryFullTaxCalculator(
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
            MunicipalityModel municipality,
            TaxPerson person,
            bool withMaxAvailableCalculationYear)
        {
            var maxCalculationYear = withMaxAvailableCalculationYear
                ? Math.Min(calculationYear, 2019)
                : calculationYear;

            var federalTaxPerson = mapper.Map<FederalTaxPerson>(person);

            var stateTaxResultTask = stateTaxCalculator.CalculateAsync(maxCalculationYear, municipality.BfsNumber, municipality.Canton, person);
            var federalTaxResultTask = federalTaxCalculator.CalculateAsync(maxCalculationYear, federalTaxPerson);

            await Task.WhenAll(stateTaxResultTask, federalTaxResultTask);

            var stateTaxResult = await stateTaxResultTask;
            var federalTaxResult = await federalTaxResultTask;

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
