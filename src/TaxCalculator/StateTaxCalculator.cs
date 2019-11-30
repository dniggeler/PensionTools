namespace TaxCalculator
{
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

    public class StateTaxCalculator : IStateTaxCalculator
    {
        private readonly IAggregatedBasisTaxCalculator basisTaxCalculator;
        private readonly IChurchTaxCalculator churchTaxCalculator;
        private readonly IPollTaxCalculator pollTaxCalculator;
        private readonly IMapper mapper;
        private readonly TaxRateDbContext dbContext;

        public StateTaxCalculator(
            IAggregatedBasisTaxCalculator basisTaxCalculator,
            IChurchTaxCalculator churchTaxCalculator,
            IPollTaxCalculator pollTaxCalculator,
            IMapper mapper,
            TaxRateDbContext dbContext)
        {
            this.basisTaxCalculator = basisTaxCalculator;
            this.churchTaxCalculator = churchTaxCalculator;
            this.pollTaxCalculator = pollTaxCalculator;
            this.mapper = mapper;
            this.dbContext = dbContext;
        }

        public async Task<Either<string, StateTaxResult>> CalculateAsync(
            int calculationYear, TaxPerson person)
        {
            string msg =
                $@"no municipality {person.Municipality} for this canton 
                    {person.Canton} and calculation {calculationYear} year found";

            var aggregatedTaxResultTask =
                 this.basisTaxCalculator.CalculateAsync(calculationYear, person);

            var pollTaxPerson = this.mapper.Map<PollTaxPerson>(person);
            var pollTaxResultTask =
                this.pollTaxCalculator.CalculateAsync(calculationYear, pollTaxPerson);

            var churchTaxPerson = this.mapper.Map<ChurchTaxPerson>(person);

            await Task.WhenAll(pollTaxResultTask, aggregatedTaxResultTask);

            var aggregatedTaxResult = await aggregatedTaxResultTask;
            var churchTaxResult = await aggregatedTaxResult
                .BindAsync(r => this.churchTaxCalculator.CalculateAsync(
                        calculationYear, churchTaxPerson, r));

            Option<TaxRateModel> taxRate = this.dbContext.Rates
                .FirstOrDefault(item => item.Canton == person.Canton &&
                                        item.Year == calculationYear &&
                                        item.Municipality == person.Municipality);
            var result = from rate in taxRate
                from r in aggregatedTaxResult.ToOption()
                from c in churchTaxResult.ToOption()
                select new StateTaxResult
                {
                    MunicipalityRate = rate.TaxRateMunicipality,
                    CantonRate = rate.TaxRateCanton,
                    BasisIncomeTax = r.IncomeTax,
                    BasisWealthTax = r.WealthTax,
                    ChurchTax = c,
                };

            result.IfSome(r => pollTaxResultTask.Result.IfRight(v => r.PollTaxAmount = v));
            return result
                .Match<Either<string, StateTaxResult>>(
                    Some: item => item,
                    None: () => msg);
        }
    }
}