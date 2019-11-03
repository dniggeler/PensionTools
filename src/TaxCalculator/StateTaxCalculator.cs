using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Tax.Data;
using Tax.Data.Abstractions.Models;

namespace TaxCalculator
{
    public class StateTaxCalculator : IStateTaxCalculator
    {
        private readonly IAggregatedBasisTaxCalculator _basisTaxCalculator;
        private readonly IPollTaxCalculator _pollTaxCalculator;
        private readonly IMapper _mapper;
        private readonly Func<TaxRateDbContext> _taxRateDbContext;

        public StateTaxCalculator(IAggregatedBasisTaxCalculator basisTaxCalculator, 
            IPollTaxCalculator pollTaxCalculator,
            IMapper mapper,
            Func<TaxRateDbContext> dbContext)
        {
            _basisTaxCalculator = basisTaxCalculator;
            _pollTaxCalculator = pollTaxCalculator;
            _mapper = mapper;
            _taxRateDbContext = dbContext;
        }

        public async Task<Either<string, TaxResult>> CalculateAsync(int calculationYear, TaxPerson person)
        {
            string msg =
                $@"no municipality {person.Municipality} for this canton 
                    {person.Canton} and calculation {calculationYear} year found";

            var aggregatedTaxResultTask = 
                 _basisTaxCalculator.CalculateAsync(calculationYear, person);

            var pollTaxPerson = _mapper.Map<PollTaxPerson>(person);
            var pollTaxResultTask =
                _pollTaxCalculator.CalculateAsync(calculationYear, pollTaxPerson);

            await Task.WhenAll(pollTaxResultTask, aggregatedTaxResultTask);

            using (var ctxt = _taxRateDbContext())
            {
                Option<TaxRateModel> taxRate = ctxt.Rates
                    .FirstOrDefault(item => item.Canton == person.Canton &&
                                            item.Year == calculationYear &&
                                            item.Municipality == person.Municipality);
                var result = from rate in taxRate
                    from r in aggregatedTaxResultTask.Result.ToOption()
                    select new TaxResult
                    {
                        CalculationYear = calculationYear,
                        MunicipalityRate = rate.TaxRateMunicipality,
                        CantonRate = rate.TaxRateCanton,
                        BasisIncomeTax = r.IncomeTax,
                        BasisWealthTax = r.WealthTax
                    };


                result.IfSome(r => pollTaxResultTask.Result.IfRight(v => r.PollTaxAmount = v));
                return result
                    .Match<Either<string, TaxResult>>(
                        Some: item => item,
                        None: () => msg);
            }

        }
    }
}