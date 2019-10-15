using System.Threading.Tasks;
using CapitalVersusPension.Abstractions;


namespace CapitalVersusPension.Calculators
{
    public class IncomeTaxCalculator : IIncomeTaxCalculator
    {
        public Task<TaxResult> CalculateAsync(TaxPerson taxPerson)
        {
            return Task.FromResult(new TaxResult
            {
                TaxRate = 0.25M,
                MunicipalTaxAmount = 8450M,
                CantonTaxAmount = 9144M,
                FederalTaxAmount = 6500M
            });
        }
    }
}