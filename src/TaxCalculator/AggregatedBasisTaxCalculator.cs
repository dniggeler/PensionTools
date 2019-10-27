using System.Threading.Tasks;
using AutoMapper;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace TaxCalculator
{
    public class AggregatedBasisTaxCalculator : IAggregatedBasisTaxCalculator
    {
        private readonly IMapper _mapper;
        private readonly IBasisIncomeTaxCalculator _basisIncomeTaxCalculator;
        private readonly IBasisWealthTaxCalculator _basisWealthTaxCalculator;

        public AggregatedBasisTaxCalculator(IMapper mapper, IBasisIncomeTaxCalculator basisIncomeTaxCalculator, 
            IBasisWealthTaxCalculator basisWealthTaxCalculator)
        {
            _mapper = mapper;
            _basisIncomeTaxCalculator = basisIncomeTaxCalculator;
            _basisWealthTaxCalculator = basisWealthTaxCalculator;
        }

        public async Task<Either<string,AggregatedBasisTaxResult>> CalculateAsync(int calculationYear, TaxPerson person)
        {
            var basisTaxPerson = _mapper.Map<BasisTaxPerson>(person);

            var incomeTaxResultTask = _basisIncomeTaxCalculator.CalculateAsync(calculationYear, basisTaxPerson);

            basisTaxPerson.TaxableAmount = person.TaxableWealth;
            var wealthTaxResultTask = _basisWealthTaxCalculator.CalculateAsync(calculationYear, basisTaxPerson);

            await Task.WhenAll(incomeTaxResultTask, wealthTaxResultTask);

            return from income in incomeTaxResultTask.Result
                from wealth in wealthTaxResultTask.Result
                select new AggregatedBasisTaxResult
                {
                    IncomeTax = income,
                    WealthTax = wealth
                };
        }
    }
}