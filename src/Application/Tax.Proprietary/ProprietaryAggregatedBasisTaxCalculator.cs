using Application.Tax.Proprietary.Contracts;
using Application.Tax.Proprietary.Models;
using AutoMapper;
using Domain.Enums;
using Domain.Models.Tax;
using Domain.Models.Tax.Person;
using LanguageExt;

namespace Application.Tax.Proprietary;

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
