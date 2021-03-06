﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator
{
    public class AggregatedBasisTaxCalculator : IAggregatedBasisTaxCalculator
    {
        private readonly IMapper mapper;
        private readonly Func<Canton, IBasisIncomeTaxCalculator> basisIncomeTaxCalculatorFunc;
        private readonly Func<Canton, IBasisWealthTaxCalculator> basisWealthTaxCalculatorFunc;

        public AggregatedBasisTaxCalculator(
            IMapper mapper,
            Func<Canton, IBasisIncomeTaxCalculator> basisIncomeTaxCalculatorFunc,
            Func<Canton, IBasisWealthTaxCalculator> basisWealthTaxCalculatorFunc)
        {
            this.mapper = mapper;
            this.basisIncomeTaxCalculatorFunc = basisIncomeTaxCalculatorFunc;
            this.basisWealthTaxCalculatorFunc = basisWealthTaxCalculatorFunc;
        }

        public async Task<Either<string, AggregatedBasisTaxResult>> CalculateAsync(
            int calculationYear, Canton canton, TaxPerson person)
        {
            var basisTaxPerson = this.mapper.Map<BasisTaxPerson>(person);

            var incomeTaxResultTask =
                this.basisIncomeTaxCalculatorFunc(canton)
                    .CalculateAsync(calculationYear, canton, basisTaxPerson);

            basisTaxPerson.TaxableAmount = person.TaxableWealth;
            var wealthTaxResultTask =
                this.basisWealthTaxCalculatorFunc(canton)
                    .CalculateAsync(calculationYear, canton, basisTaxPerson);

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