﻿namespace TaxCalculator
{
    using System.Threading.Tasks;
    using AutoMapper;
    using LanguageExt;
    using PensionCoach.Tools.TaxCalculator.Abstractions;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

    public class AggregatedBasisTaxCalculator : IAggregatedBasisTaxCalculator
    {
        private readonly IMapper mapper;
        private readonly IBasisIncomeTaxCalculator basisIncomeTaxCalculator;
        private readonly IBasisWealthTaxCalculator basisWealthTaxCalculator;

        public AggregatedBasisTaxCalculator(
            IMapper mapper, IBasisIncomeTaxCalculator basisIncomeTaxCalculator, IBasisWealthTaxCalculator basisWealthTaxCalculator)
        {
            this.mapper = mapper;
            this.basisIncomeTaxCalculator = basisIncomeTaxCalculator;
            this.basisWealthTaxCalculator = basisWealthTaxCalculator;
        }

        public async Task<Either<string,AggregatedBasisTaxResult>> CalculateAsync(int calculationYear, TaxPerson person)
        {
            var basisTaxPerson = this.mapper.Map<BasisTaxPerson>(person);

            var incomeTaxResultTask = 
                this.basisIncomeTaxCalculator.CalculateAsync(calculationYear, basisTaxPerson);

            basisTaxPerson.TaxableAmount = person.TaxableWealth;
            var wealthTaxResultTask = 
                this.basisWealthTaxCalculator.CalculateAsync(calculationYear, basisTaxPerson);

            await Task.WhenAll(incomeTaxResultTask, wealthTaxResultTask);

            return from income in incomeTaxResultTask.Result
                from wealth in wealthTaxResultTask.Result
                select new AggregatedBasisTaxResult
                {
                    IncomeTax = income,
                    WealthTax = wealth,
                };
        }
    }
}