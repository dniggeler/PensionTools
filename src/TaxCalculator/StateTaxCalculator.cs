using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.EntityFrameworkCore.Storage;
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

        public StateTaxCalculator(IAggregatedBasisTaxCalculator basisTaxCalculator, TaxRateDbContext dbContext)
        {
            _basisTaxCalculator = basisTaxCalculator;
            _taxRateDbContext = dbContext;
        }

        public async Task<Either<string, TaxResult>> CalculateAsync(int calculationYear, TaxPerson person)
        {
            string msg =
                $"No municipality {person.Municipality} for this canton {person.Canton} and calculation {calculationYear} year found.";

            var aggregatedTaxResult = await _basisTaxCalculator.CalculateAsync(calculationYear, person);
            var taxRate = _taxRateDbContext.Rates
                .Where(item => item.Canton == person.Canton &&
                               item.Year == person.CalculationYear &&
                               item.Municipality == person.Municipality)
                .ToList()
                .Match<Either<string,TaxRateModel>>(
                    Empty: () => msg,
                    More: s => "Query not unique",
                    One: item => item);

            return new TaxResult
            {
                CalculationYear = person.CalculationYear,
                ReferencedTaxableAmount = 0,
                BaseTaxAmount = 0,
                MunicipalityRate = taxRate.TaxRateMunicipality,
                CantonRate = taxRate.TaxRateCanton,
            };

        }
    }
}