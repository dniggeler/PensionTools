using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using Tax.Data;
using Tax.Data.Abstractions.Models;

namespace TaxCalculator
{
    public class StateTaxCalculator : IStateTaxCalculator
    {
        private readonly IAggregatedBasisTaxCalculator _basisTaxCalculator;
        private readonly TaxRateDbContext _taxRateDbContext;

        public StateTaxCalculator(IAggregatedBasisTaxCalculator basisTaxCalculator, 
            TaxRateDbContext dbContext)
        {
            _basisTaxCalculator = basisTaxCalculator;
            _taxRateDbContext = dbContext;
        }

        public async Task<Either<string, TaxResult>> CalculateAsync(int calculationYear, TaxPerson person)
        {
            string msg =
                $@"no municipality {person.Municipality} for this canton 
                    {person.Canton} and calculation {calculationYear} year found";

            Either<string, AggregatedBasisTaxResult> aggregatedTaxResult = 
                await _basisTaxCalculator.CalculateAsync(calculationYear, person);

            Option<TaxRateModel> taxRate = _taxRateDbContext.Rates
                .FirstOrDefault(item => item.Canton == person.Canton &&
                                        item.Year == calculationYear &&
                                        item.Municipality == person.Municipality);
            var result = from rate in taxRate
                from r in aggregatedTaxResult.ToOption()
                select new TaxResult
                {
                    CalculationYear = calculationYear,
                    MunicipalityRate = rate.TaxRateMunicipality,
                    CantonRate = rate.TaxRateCanton,
                    BasisIncomeTax = r.IncomeTax,
                    BasisWealthTax = r.WealthTax
                };

            return result
                .Match<Either<string, TaxResult>>(
                    Some: item => item,
                    None: () => msg);
        }
    }
}