using System.Threading.Tasks;
using AutoMapper;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

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
            int calculationYear, TaxPerson person)
        {
            var federalTaxPerson = this.mapper.Map<FederalTaxPerson>(person);

            var stateTaxResultTask = this.stateTaxCalculator.CalculateAsync(calculationYear, person);
            var federalTaxResultTask = this.federalTaxCalculator.CalculateAsync(calculationYear, federalTaxPerson);

            await Task.WhenAll(stateTaxResultTask, federalTaxResultTask);

            return
                (from stateTax in stateTaxResultTask.Result.ToOption()
                 from federalTax in federalTaxResultTask.Result.ToOption()
                 select new FullTaxResult()
                 {
                     StateTaxResult = stateTax,
                     FederalTaxResult = federalTax,
                 })
                .Match<Either<string, FullTaxResult>>(
                    Some: v => v,
                    None: () => "Full tax calculation failed");
        }
    }
}