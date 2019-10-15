using System;
using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;


namespace TaxCalculator
{
    public class IncomeTaxCalculator : IIncomeTaxCalculator
    {
        public Task<Either<TaxResult, string>> CalculateAsync(TaxPerson person)
        {
            Option<TaxResult> result = from i in person.TaxableIncome
                select (new TaxResult
                {
                    TaxableIncome = i,
                    Rate = 0.02M,
                });
                    
            return Task.FromResult( result
                .Match<Either<TaxResult,string>>(
                    Some: r => r,
                    None: () => "incomplete input data"));
        }
    }
}
