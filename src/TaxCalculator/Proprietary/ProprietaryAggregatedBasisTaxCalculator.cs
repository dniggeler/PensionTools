using System;
using System.Threading.Tasks;
using AutoMapper;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace PensionCoach.Tools.TaxCalculator.Proprietary;

public class ProprietaryAggregatedBasisTaxCalculator : IAggregatedBasisTaxCalculator
{
    private readonly IMapper mapper;
    private readonly Func<Canton, IBasisIncomeTaxCalculator> basisIncomeTaxCalculatorFunc;
    private readonly Func<Canton, IBasisWealthTaxCalculator> basisWealthTaxCalculatorFunc;

    public ProprietaryAggregatedBasisTaxCalculator(
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
        var basisTaxPerson = mapper.Map<BasisTaxPerson>(person);

        var incomeTaxResultTask =
            basisIncomeTaxCalculatorFunc(canton)
                .CalculateAsync(calculationYear, canton, basisTaxPerson);

        basisTaxPerson.TaxableAmount = person.TaxableWealth;
        var wealthTaxResultTask =
            basisWealthTaxCalculatorFunc(canton)
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
